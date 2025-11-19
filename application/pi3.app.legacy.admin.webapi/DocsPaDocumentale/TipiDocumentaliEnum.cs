// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaDocumentale
{
    /// <summary>
    /// Enumerazione di tutti i software documentali esterni utilizzati da docspa
    /// </summary>
    public enum TipiDocumentaliEnum
    {
        Etnoteam,
        Hummingbird,
        Filenet,
        Pitre,
        CDC,

        GFD,
        SharePoint
    }
}
