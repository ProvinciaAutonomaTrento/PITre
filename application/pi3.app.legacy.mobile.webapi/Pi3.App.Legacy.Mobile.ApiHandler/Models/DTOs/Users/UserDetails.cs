// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.Text.Json.Serialization;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs.Users;
public class UserDetails : User
{
    [JsonPropertyName("Descrizione")]
    public string? FullName { get; internal set; }
    public string? Dst { get; set; }
    public IEnumerable<Roles.RoleDetails>? Ruoli { get; set; }
    public string? Token { get; set; }

}
