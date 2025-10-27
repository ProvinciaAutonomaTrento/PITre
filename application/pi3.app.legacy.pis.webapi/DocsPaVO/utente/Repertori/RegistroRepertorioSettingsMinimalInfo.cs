// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.Repertori
{
    [Serializable()]
    public class RegistroRepertorioSettingsMinimalInfo
    {
        public RegistroRepertorio.SettingsType Settings { get; set; }
        public String RegistryOrRfDescription { get; set; }
        public String RoleAndUserDescription { get; set; }
        public String RegistryId { get; set; }
        public String RfId { get; set; }
        public RegistroRepertorioSingleSettings.RepertorioState CounterState { get; set; }
    }
}
