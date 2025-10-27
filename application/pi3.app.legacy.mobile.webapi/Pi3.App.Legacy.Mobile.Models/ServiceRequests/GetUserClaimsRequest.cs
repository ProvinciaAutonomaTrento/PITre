// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Mobile.Models.ServiceRequests;

public class GetUserClaimsRequest
{
    public string? Username { get; set; }
    public long GroupId { get; set; }
    public long IdAmministrazione { get; set; }
}