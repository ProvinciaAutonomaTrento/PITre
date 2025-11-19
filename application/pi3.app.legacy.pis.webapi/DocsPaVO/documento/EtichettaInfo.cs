// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.documento
{
    /// <summary>
    /// commento
    /// </summary>
    [Serializable()]
    public class EtichettaInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Codice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Descrizione { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int IdAmministrazione { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Etichetta { get; set; }
    }
}
