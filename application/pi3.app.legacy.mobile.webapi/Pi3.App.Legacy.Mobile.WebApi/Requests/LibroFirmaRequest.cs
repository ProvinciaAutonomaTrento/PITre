// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Mobile.WebApi.Services.RabbitMQ;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Mobile.WebApi.Requests
{
    public class LibroFirmaRequest : MessageQueueBaseCommand
    {
        public LibroFirmaRequest()
        : base()
        { }

        public LibroFirmaRequest(System.Security.Claims.ClaimsPrincipal claimsPrincipal)
            : base(claimsPrincipal)
        { }

        [Required]
        public string IdProfile { get; init; } = null!;

        [Required]
        public string Evento { get; init; } = null!;
    }
}
