// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.Models.ServiceDtos;
public class AuthenticationResult
{
    public string? UserId { get; internal set; }
    public long? IdAmministrazione { get; internal set; }
    public long? IdPeople { get; internal set; }
    public string? Descrizione { get; internal set; }
    public Mobile.Shared.Exceptions.Internal.LoginResponseCode Code { get; set; }
    public string? Dst { get; set; }
    public string? Memento { get; set; }
    // public IEnumerable<SelectTemplates.RuoloUtente>? Ruoli { get; set; }
}
