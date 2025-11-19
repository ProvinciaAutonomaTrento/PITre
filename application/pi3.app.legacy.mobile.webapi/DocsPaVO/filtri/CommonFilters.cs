// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.filtri
{
    /// <summary>
    /// Filtri comuni a tutte le ricerche
    /// </summary>
    public enum CommonSearchFilters
    {
        ID_OWNER,
        DESC_OWNER,
        EXTEND_TO_HISTORICIZED_OWNER,
        CORR_TYPE_OWNER,
        ID_AUTHOR,
        DESC_AUTHOR,
        EXTEND_TO_HISTORICIZED_AUTHOR,
        CORR_TYPE_AUTHOR
    }
}
