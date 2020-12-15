using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Import.AddressBook
{
    public class ImportAddressBookException :ImportException
    {
        /// <summary>
        /// Funzione per l'inizializzazione di un'eccezione rilevata durante l'importazione 
        /// di corrispondenti in rubrica
        /// </summary>
        /// <param name="message">Il messaggio da attribuire all'eccezione</param>
        public ImportAddressBookException(string message) : base(message) { }
    }
}
