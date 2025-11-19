// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.amministrazione;

namespace DocsPaVO.RubricaComune
{
    /// <summary>
    /// Rappresenta le informazioni di una uo docspa da inserire nella rubrica comune
    /// </summary>
    [Serializable()]
    public class ElementoRubricaUO : ElementoRC
    {
        /// <summary>
        /// Rappresenta l'Uo docspa da inviare in rubrica comune
        /// </summary>
        public OrgUO UO;
    }
}
