// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente
{
    /// <summary>
    /// Questo oggetto rappresenta le informaizoni minimali relative ad un utente
    /// </summary>
    public class UserMinimalInfo
    {
        /// <summary>
        /// Id dell'utente
        /// </summary>
        public String SystemId { get; set; }

        /// <summary>
        /// Descrizione del corrispondente
        /// </summary>
        public String Description { get; set; }
    }
}
