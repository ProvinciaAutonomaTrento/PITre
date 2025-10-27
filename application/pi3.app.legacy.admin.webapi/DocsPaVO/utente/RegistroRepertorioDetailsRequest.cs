// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente
{
    /// <summary>
    /// Richiesta per il metodo web di recupero del dettaglio di un registro di repertorio
    /// </summary>
    [Serializable()]
    public class RegistroRepertorioDetailsRequest
    {
        /// <summary>
        /// Id del registro di repertorio
        /// </summary>
        public String RegistryId { get; set; }
    }
}
