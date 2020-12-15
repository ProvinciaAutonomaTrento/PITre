using System;

namespace DocsPaVO.Import.AddressBook
{
    /// <summary>
    /// Questa classe rappresenta i dati estratti da una riga
    /// di foglio Excel.
    /// </summary>
    [Serializable()]
    public class AddressBookRowData
    {
        /// <summary>
        /// Enumerazione del tipo di corrispondente su cui compiere le operazioni
        /// </summary>
        public enum CorrEnum
        {
            U,          // Unità organizzativa
            P,          // Persona
            R           // Ruolo
        }

        /// <summary>
        /// Il tipo di corrispondente su cui compiere l'operazione
        /// </summary>
        public CorrEnum CorrType { get; set; }

        /// <summary>
        /// Codice del registro.
        /// </summary>
        public string RegistryCode { get; set; }

        /// <summary>
        /// Codice rubrica del corrispondente.
        /// </summary>
        public string AddressBookCode { get; set; }

        /// <summary>
        /// Codice dell'amministrazione.
        /// </summary>
        public string AdministrationCode { get; set; }

        /// <summary>
        /// Codice dell'Area Organizzativa Omogena
        /// </summary>
        public string AOOCode { get; set; }

        /// <summary>
        /// Descrizione del corrispondente.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Cognome del corrispondente.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Nome del corrispondente.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indirizzo del corrispondente.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Codice di avviamento postale del corrispondente.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Città.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Provincia.
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// Nazione.
        /// </summary>
        public string Nation { get; set; }

        /// <summary>
        /// Codice fiscale.
        /// </summary>
        public string FiscalCode { get; set; }

        /// <summary>
        /// Partita iva.
        /// </summary>
        public string VatNumber { get; set; }

        /// <summary>
        /// Numero di telefono primario.
        /// </summary>
        public string PhoneNumber1 { get; set; }

        /// <summary>
        /// Numero di telefono secondario.
        /// </summary>
        public string PhoneNumber2 { get; set; }

        /// <summary>
        /// Numero di fax.
        /// </summary>
        public string FaxNumber { get; set; }

        /// <summary>
        /// Indirizzo email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Registro cui è associato il corrispondente. Questo campo viene preso in considerazione
        /// in caso di modifica del corrispondente e serve per lo spostamento del corrispondente dal
        /// registro speicifcato in questo campo al registro specificato nel campo RegistryCode.
        /// </summary>
        public string FromRegistry { get; set; }

        /// <summary>
        /// Il nome del canale preferenziale.
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Località
        /// </summary>
        public string Localita { get; set; }

    }
}
