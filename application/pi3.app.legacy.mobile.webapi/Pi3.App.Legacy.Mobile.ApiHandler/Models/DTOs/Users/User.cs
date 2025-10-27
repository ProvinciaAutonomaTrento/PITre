// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs.Users;
public class User
{
    public long IdPeople { get; internal set; }
    public string? UserId { get; internal set; }
    public long? IdAmministrazione { get; internal set; }
}
