// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.fascicolazione
{
    /// <summary>
    /// Informazioni di base su di un fascicolo o sottofascicolo
    /// </summary>
    [Serializable()]
    public class FascOrFolderBaseInfo
    {
        public enum TypeEnum
        {
            Fascicolo,
            Folder
        }

        public String Id { get; set; }

        public TypeEnum Type { get; set; }

    }
}
