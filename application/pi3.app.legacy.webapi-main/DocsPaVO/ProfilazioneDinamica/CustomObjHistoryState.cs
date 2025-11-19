// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;

namespace DocsPaVO.ProfilazioneDinamica
{
    /// <summary>
    /// Informazioni relative ad un campo dello storico dei campi profilati
    /// </summary>
    [Serializable()]
    public class CustomObjHistoryState
    {
        /// <summary>
        /// Identificativo del campo
        /// </summary>
        public int FieldId { get; set; }

        /// <summary>
        /// Descrizione del campo
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Flag che indica se è attivo lo storico per questo campo
        /// </summary>
        public bool Enabled { get; set; }
    }
}
