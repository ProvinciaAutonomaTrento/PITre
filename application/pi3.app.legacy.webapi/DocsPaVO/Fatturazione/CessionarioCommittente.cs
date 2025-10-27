// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Fatturazione
{
    [Serializable()]
    public class CessionarioCommittente
    {
        // Dati anagrafici
        public string idPaese;
        public string idCodiceI;
        public string idCodiceF;
        public string denominazione;

        // Sede
        public string indirizzo;
        public string numCivico;
        public string CAP;
        public string comune;
        public string provincia;
        public string nazione;
    }
}
