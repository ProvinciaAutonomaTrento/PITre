// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    /// <summary>
    /// Request relativa al servizio di recupero delle imformazioni di base sugli utenti di un ruolo
    /// </summary>
    [DataContract]
    public class GetUsersInRoleMinimalInfoResponse
    {
        /// <summary>
        /// Lista delle informazioni di base sugli utenti
        /// </summary>
        [DataMember]
        public List<UserMinimalInfo> Users { get; set; }
    }
}
