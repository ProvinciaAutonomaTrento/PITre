// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.RegistroConservazione
{

    /// <summary>
    /// Contiene le informazioni necessarie per la stampa automatica
    /// del registro di conservazione.
    /// </summary>
    [Serializable()]
    public class RegistroConservazionePrint
    {
        public string idAmministrazione { get; set; }
        public string printFreq { get; set; } //usare un enum?
        public string idLastPrinted { get; set; }
        public string idLastToPrint { get; set; }
        public DateTime lastPrintDate { get; set; }
        public DateTime nextPrintDate { get; set; }
        public string print_userId { get; set; }
        public string print_role { get; set; }
        public int printHour { get; set; }

    }
}
