// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Mobile.Models.ServiceRequests;
public class AuthenticateUserRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public long? IdAmministrazione { get; set; }
}
