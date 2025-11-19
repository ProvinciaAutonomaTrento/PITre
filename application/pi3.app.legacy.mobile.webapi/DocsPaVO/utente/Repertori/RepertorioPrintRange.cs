// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;

namespace DocsPaVO.utente.Repertori
{
    /// <summary>
    /// Intervallo di stampa relativo ad un repertorio.
    /// </summary>
    [Serializable()]
    public class RepertorioPrintRange
    {
        /// <summary>
        /// Anno cui si riferisce l'intervallo di stampa
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Estremo inferiore del range
        /// </summary>
        public int FirstNumber { get; set; }

        /// <summary>
        /// Estremo inferiore del range
        /// </summary>
        public int LastNumber { get; set; }

    }
}
