// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DocsPaVO.Note
{
    [Serializable()]
    public class NotaElenco
    {
        public string idNota = string.Empty;
        public string codRegRf = string.Empty;
        public string descNota = string.Empty;
        public string idRegRf = string.Empty;
    }
}
