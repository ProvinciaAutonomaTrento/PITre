// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    [Serializable()]
    public class ChangeRepertorioStateRequest
    {

        public string CounterId { get; set; }

        public string RegistryId { get; set; }

        public string RfId { get; set; }

        public string IdAmm { get; set; }
    }
}
