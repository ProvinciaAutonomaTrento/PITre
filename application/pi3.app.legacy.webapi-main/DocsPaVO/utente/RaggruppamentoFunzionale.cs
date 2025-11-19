// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.amministrazione;

namespace DocsPaVO.utente
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RaggruppamentoFunzionale : Corrispondente
    {
        //public String Id { get; set; }
        public String Codice { get; set; }
        //public String Descrizione { get; set; }
        //public OrgDettagliGlobali Dettagli { get; set; }
        //public String IdAmministrazione { get; set; }
    }
}