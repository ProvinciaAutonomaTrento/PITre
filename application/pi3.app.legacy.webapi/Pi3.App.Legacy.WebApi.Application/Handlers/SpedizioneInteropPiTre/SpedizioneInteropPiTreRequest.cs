// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Legacy.WebApi.Application.Services.Interoperability;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SpedizioneInteropPiTre
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
