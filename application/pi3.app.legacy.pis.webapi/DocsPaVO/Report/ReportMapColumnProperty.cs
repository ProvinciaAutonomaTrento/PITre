// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;

namespace DocsPaVO.Report
{
    /// <summary>
    /// Questo oggetto rappresenta il mapping fra un elemento di una sorgente dati
    /// ed una riga del report
    /// </summary>
    [Serializable()]
    public class ReportMapColumnProperty : HeaderProperty
    {
        
        /// <summary>
        /// Valore da esportare
        /// </summary>
        public String Value { get; set; }

    }
}
