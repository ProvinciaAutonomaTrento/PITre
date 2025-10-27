// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Modelli_Trasmissioni
{
    /// <summary>
    /// Response del servizio di find and replace relativo ai ruoli dei modelli
    /// di trasmissione
    /// </summary>
    [Serializable()]
    public class FindAndReplaceResponse
    {
        /// <summary>
        /// Lista dei modelli
        /// </summary>
        public ModelloTrasmissioneSearchResultCollection Models { get; set; }

    }
}
