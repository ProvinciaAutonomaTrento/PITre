// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace DocsPaVO.documento
{
    /// <summary>
    /// Possibili risultati di una fascicolazione.
    /// </summary>

    [XmlType("ResultFascicolazione")]
    public enum ResultFascicolazione
    {
        OK,
        DOCUMENTO_IN_LIBRO_FIRMA_PASSO_NON_ATTESO
    }

}
