// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs.Roles;
public class RoleDetails
{
    public long? Id { get; set; }
    public string? Descrizione { get; set; }
    public long? IdGruppo { get; set; }
    public string? Codice { get; set; }
    public string? Livello { get; set; }
    public string? IdUO { get; set; }
    public List<RegisterDetails>? Registri { get; set; }

}
