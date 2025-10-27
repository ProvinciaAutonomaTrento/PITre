// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.documento
{
    [Serializable()]
    public class FascicolazioneTipiDocumento
    {
        /// <summary>
        /// 
        /// </summary>
        public string Codice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool FascicolazioneObbligatoria { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IdAmministrazione { get; set; }
    }
}
