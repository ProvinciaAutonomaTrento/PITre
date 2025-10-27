// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GiveUpRights
{
    public class GiveUpRightsCommand : IRequest<GiveUpRightsCommandResponse>
    {
        [Required]
        public string RightToKeep { get; set; }
        [Required]
        public string IdObject { get; set; }
    }

    public class GiveUpRightsCommandResponse : MessageResponse
    {
    }
}
