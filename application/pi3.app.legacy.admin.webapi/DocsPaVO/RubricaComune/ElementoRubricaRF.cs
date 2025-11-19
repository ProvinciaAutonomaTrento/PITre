// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using DocsPaVO.utente;

namespace DocsPaVO.RubricaComune
{
    /// <summary>
    /// Informazioni sull'RF da inviare a Rubrica Comune
    /// </summary>
    [Serializable]
    [DataContract]
    public class ElementoRubricaRF : ElementoRC
    {
        /// <summary>
        /// Raggruppamento Funzionale da inviare a Rubrica Comune
        /// </summary>
        [DataMember]
        public RaggruppamentoFunzionale RF { get; set; }
    }
}
