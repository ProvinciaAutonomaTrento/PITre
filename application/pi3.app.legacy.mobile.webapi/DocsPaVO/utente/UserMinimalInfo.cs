// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.utente
{
    /// <summary>
    /// Questo oggetto rappresenta le informaizoni minimali relative ad un utente
    /// </summary>
    [DataContract]
    public class UserMinimalInfo
    {
        /// <summary>
        /// Id dell'utente
        /// </summary>
        [DataMember]
        public String SystemId { get; set; }

        /// <summary>
        /// Descrizione del corrispondente
        /// </summary>
        [DataMember]
        public String Description { get; set; }
    }
}
