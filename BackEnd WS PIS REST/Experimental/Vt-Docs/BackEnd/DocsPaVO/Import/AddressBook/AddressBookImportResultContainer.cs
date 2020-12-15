using System;
using System.Collections.Generic;

namespace DocsPaVO.Import.AddressBook
{
    /// <summary>
    /// Contenitore dei risultati di importazione dei corrispondenti
    /// </summary>
    [Serializable()]
    public class AddressBookImportResultContainer
    {
        /// <summary>
        /// Metodo per la creazione e l'inizializzazione del report
        /// </summary>
        public AddressBookImportResultContainer()
        {
            this.Inserted = new List<AddressBookImportResult>();
            this.Modified = new List<AddressBookImportResult>();
            this.Deleted = new List<AddressBookImportResult>();
            this.General = new List<AddressBookImportResult>();

        }

        /// <summary>
        /// Informazioni generali
        /// </summary>
        public List<AddressBookImportResult> General { get; set; }

        /// <summary>
        /// Corrispondenti inseriti
        /// </summary>
        public List<AddressBookImportResult> Inserted { get; set; }

        /// <summary>
        /// Corrispondenti modificati
        /// </summary>
        public List<AddressBookImportResult> Modified { get; set; }

        /// <summary>
        /// Corrispondenti cancellati
        /// </summary>
        public List<AddressBookImportResult> Deleted { get; set; }
    
    }
}
