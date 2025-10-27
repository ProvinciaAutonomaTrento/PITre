// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Legacy.Mobile.WebApi.Services.InteroperabilityService;
using Pi3.App.Legacy.Mobile.WebApi.Services.RabbitMQ;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers.Spedizione.InteroperabilitaSemplificata
{
    public class SpedizioneInteropPiTreRequest : MessageQueueBaseCommand
    {
        public SpedizioneInteropPiTreRequest()
        : base()
        { }

        public SpedizioneInteropPiTreRequest(System.Security.Claims.ClaimsPrincipal claimsPrincipal)
            : base(claimsPrincipal)
        { }

        [Required]
        public string Instance { get; init; } = null!;

        [Required]
        public string Authorization { get; init; } = null!;

        [Required]
        public string Tenant { get; init; } = null!;

        [Required]
        public InteroperabilityMessage InteroperabilityMessage { get; init; } = null!;
    }
}
