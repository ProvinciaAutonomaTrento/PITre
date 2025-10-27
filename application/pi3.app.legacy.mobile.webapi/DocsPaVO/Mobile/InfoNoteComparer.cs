// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Note;

namespace DocsPaVO.Mobile
{
    public class InfoNoteComparer : IComparer<InfoNota>
    {
        public int Compare(InfoNota x, InfoNota y)
        {
            if (x.DataCreazione == null) return -1;
            if (y.DataCreazione == null) return 1;
            return x.DataCreazione.CompareTo(y.DataCreazione);
        }
    }
}
