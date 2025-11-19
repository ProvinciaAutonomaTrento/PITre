// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Grid
{
    [Serializable()]
    public class FieldValue
    {
        /// <summary>
        /// Valore
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Colore
        /// </summary>
        public string ColorBG{ get; set; }
    }
}
