// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.Collections.Generic;

namespace DocsPaVO.Import.AddressBook
{
    /// <summary>
    /// Classe utilizzata per ocntenere i dati estratti da un foglio Excel
    /// riguardanti dati su corrispondenti da inserire, modificare e cancellare
    /// </summary>
    public class AddressBookRowDataContainer
    {
        /// <summary>
        /// La lista dei corrispondenti da inserire
        /// </summary>
        public List<AddressBookRowData> ToInsert;

        /// <summary>
        /// La lista dei corrispondenti da modificare
        /// </summary>
        public List<AddressBookRowData> ToModify;

        /// <summary>
        /// La lista dei corrispondenti da eliminare
        /// </summary>
        public List<AddressBookRowData> ToDelete;
    }
}
