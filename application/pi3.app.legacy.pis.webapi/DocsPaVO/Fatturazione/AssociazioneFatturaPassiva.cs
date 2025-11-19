// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Fatturazione
{
    [Serializable]
    public class AssociazioneFatturaPassiva
    {
        public String IdFattura { get; set; }

        public String IdSdi { get; set; }

        public String Docnumber { get; set; }
    }
}
