// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente
{
    /// <summary>
    /// Request relativa al servizio di richiesta della storia delle storicizzazioni di un ruolo
    /// </summary>
    [Serializable()]
    public class RoleHistoryRequest
    {
        public String IdCorrGlobRole { get; set; }
    }
}
