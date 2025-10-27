// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Legacy.Mobile.Models.SelectTemplates;

namespace Pi3.App.Legacy.Mobile.Models.ServiceDtos;
public class UserClaims
{
    public UserDetails? UserDetails { get; set; }
    public IEnumerable<string>? Functions { get; set; }
}
