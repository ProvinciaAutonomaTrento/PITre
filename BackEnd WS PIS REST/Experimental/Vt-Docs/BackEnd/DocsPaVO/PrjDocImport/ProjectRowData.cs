using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Note;
using System.Xml.Serialization;

namespace DocsPaVO.PrjDocImport
{
    /// <summary>
    /// Classe che rappresenta i dati descrittivi di un fascicolo 
    /// estratti da una sorgente dati.
    /// </summary>
    [Serializable()]
    public class ProjectRowData
    {
        /// <summary>
        /// Creazione di un nuovo oggetto con i dati estratti da una riga
        /// </summary>
        public ProjectRowData()
        {
            this.ProfilationData = new List<ProfilationFieldInformation>();
        }

        /// <summary>
        /// L'ordinale
        /// </summary>
        public string OrdinalNumber { get; set; }

        /// <summary>
        /// Il codice dell'amministrazione
        /// </summary>
        public string AdminCode { get; set; }

        /// <summary>
        /// Il codice del registro
        /// </summary>
        public string RegistryCode { get; set; }

        /// <summary>
        /// Il codice dell'RF
        /// </summary>
        public string RFCode { get; set; }

        /// <summary>
        /// La descrizione del fascicolo
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// La descrizione del titolario in cui creare il fascicolo
        /// </summary>
        //public string Titolario { get; set; }

        /// <summary>
        /// Il codice del nodo
        /// </summary>
        public string NodeCode { get; set; }

        /// <summary>
        /// Il numero del fascicolo
        /// </summary>
        public string ProjectNumber { get; set; }

        /// <summary>
        /// La data di creazione
        /// </summary>
        //public string CreationDate { get; set; }

        /// <summary>
        /// True se il fascicolo và salvato nell'area di lavoro
        /// </summary>
        public bool InWorkingArea { get; set; }

        /// <summary>
        /// Le note da associare al fascicolo
        /// </summary>
        public InfoNota Note { get; set; }

        /// <summary>
        /// I codici dei modelli di trasmissione
        /// </summary>
        public string[] TransmissionModelCode { get; set; }

        /// <summary>
        /// La tipologia di fascicolo
        /// </summary>
        public string ProjectTipology { get; set; }

        /// <summary>
        /// L'oggetto in cui vengono memorizzate le infomrazioni
        /// sui campi profilati (etichetta e lista valori)
        /// </summary>
        public List<ProfilationFieldInformation> ProfilationData { get; set; }

        /// <summary>
        /// Funzione per l'aggiunta di un campo alla lista dei campi
        /// </summary>
        /// <param name="fieldName">L'etichetta da associare al campo</param>
        /// <param name="values">L'insieme dei valori da associare al campo</param>
        public void AddProfilationField(string fieldName, string[] values)
        {
            this.ProfilationData.Add(new ProfilationFieldInformation()
            {
                Label = fieldName.ToUpper().Trim(),
                Values = values
            });
        }

        /// <summary>
        /// Funzione per la ricerca di un campo profilato all'interno della lista
        /// dei campi profilati legati al fascicolo
        /// </summary>
        /// <param name="fieldLabel">La label del campo di cui si desidera ricevere i dati</param>
        /// <returns>L'insieme dei valori assegnati al campo</returns>
        public string[] GetProfilationField(string fieldLabel)
        {
            // Le informazioni sul campo richiesto
            ProfilationFieldInformation fieldInformation = null;

            // L'oggetto da restituire
            string[] toReturn = null;

            // Selezione dell'elemento
            fieldInformation = this.ProfilationData.Where(
                e => e.Label == fieldLabel.ToUpper()).FirstOrDefault();

            if (fieldInformation != null)
                toReturn = fieldInformation.Values;

            // Restituzione valore
            return toReturn;

        }

        public override string ToString()
        {
            return String.Format("Ordinale: {0} - Numero : {1}",
                this.OrdinalNumber, this.ProjectNumber);
        }

    }
}
