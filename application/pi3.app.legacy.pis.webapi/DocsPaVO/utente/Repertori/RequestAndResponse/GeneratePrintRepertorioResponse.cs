// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.documento;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GeneratePrintRepertorioResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public SchedaDocumento Document { get; set; }
    }
}
