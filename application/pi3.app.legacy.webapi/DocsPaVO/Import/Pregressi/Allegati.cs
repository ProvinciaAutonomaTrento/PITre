// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Import.Pregressi
{
    [Serializable()]
    public class Allegati
    {
        public string systemId = string.Empty;
        public string idItem = string.Empty;
        public string errore = string.Empty;
        public string esito = string.Empty;
        public string ordinale = string.Empty;
        public string descrizione = string.Empty;
        public string pathname = string.Empty;

        public Allegati()
        { }
    }
}
