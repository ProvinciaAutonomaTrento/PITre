// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
