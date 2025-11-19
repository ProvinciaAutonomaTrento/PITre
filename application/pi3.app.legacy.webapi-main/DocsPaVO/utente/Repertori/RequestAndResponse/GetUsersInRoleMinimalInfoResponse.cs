// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    /// <summary>
    /// Request relativa al servizio di recupero delle imformazioni di base sugli utenti di un ruolo
    /// </summary>
    public class GetUsersInRoleMinimalInfoResponse
    {
        /// <summary>
        /// Lista delle informazioni di base sugli utenti
        /// </summary>
        public List<UserMinimalInfo> Users { get; set; }
    }
}
