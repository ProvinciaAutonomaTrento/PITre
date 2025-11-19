// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Mobile.WebApi.Services.RabbitMQ;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers.LibroFirma.Requests
{
    public class ConvertVersionToPdfRequest : MessageQueueBaseCommand
    {
        public ConvertVersionToPdfRequest()
        : base()
        { }

        public ConvertVersionToPdfRequest(System.Security.Claims.ClaimsPrincipal claimsPrincipal)
            : base(claimsPrincipal)
        { }

        [Required]
        public string Id { get; init; } = null!;

        public string? IdVersion { get; init; }
    }
}
