// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;

namespace DocsPaVO.utente
{
    /// <summary>
    /// Response relativa al servizio di recupero della storia delle storicizzazioni di un ruolo
    /// </summary>
    [Serializable()]
    public class RoleHistoryResponse
    {
        public RoleHistoryResponse()
        {
            this.RoleHistoryItems = new List<RoleHistoryItem>();
        }

        /// <summary>
        /// Item costuenti la storia delle modifiche apportate al ruolo
        /// </summary>
        public List<RoleHistoryItem> RoleHistoryItems { get; set; }
        
    }
}
