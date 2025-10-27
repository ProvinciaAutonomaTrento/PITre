// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GeneraMetadatiAGID
{
    public class GeneraMetadatiAGIDRequest : MessageQueueBaseCommand
    {
        public GeneraMetadatiAGIDRequest()
        : base()
        { }

        public GeneraMetadatiAGIDRequest(System.Security.Claims.ClaimsPrincipal claimsPrincipal)
            : base(claimsPrincipal)
        { }

        [Required]
        public string Method { get; init; }

        [Required]
        public long IdOggetto { get; init; }

        [Required]
        public string DescrizioneOggetto { get; init; }

        [Required]
        public  DateTime DataAzione { get; init; }

        public long? IdTrasmissione { get; init; } = null!;
    }
}
