// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Interoperability;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SpedizioneInteropPiTre
{
    public class SpedizioneInteropPiTreCommand : MessageQueueBaseCommand
    {
        public SpedizioneInteropPiTreCommand()
        : base()
        { }

        public SpedizioneInteropPiTreCommand(System.Security.Claims.ClaimsPrincipal claimsPrincipal)
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
