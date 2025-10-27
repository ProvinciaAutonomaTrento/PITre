// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Runtime.Serialization;

namespace DocsPaVO.ProfilazioneDinamica
{
    /// <summary>
    /// Informazioni relative ad un campo dello storico dei campi profilati
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CustomObjHistoryState
    {
        /// <summary>
        /// Identificativo del campo
        /// </summary>
        [DataMember]
        public int FieldId { get; set; }

        /// <summary>
        /// Descrizione del campo
        /// </summary>
        [DataMember]
        public String Description { get; set; }

        /// <summary>
        /// Flag che indica se è attivo lo storico per questo campo
        /// </summary>
        [DataMember]
        public bool Enabled { get; set; }
    }
}
