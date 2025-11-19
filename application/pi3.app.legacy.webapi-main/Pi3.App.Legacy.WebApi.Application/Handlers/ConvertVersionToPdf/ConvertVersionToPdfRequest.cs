// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ConvertVersionToPdf
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
