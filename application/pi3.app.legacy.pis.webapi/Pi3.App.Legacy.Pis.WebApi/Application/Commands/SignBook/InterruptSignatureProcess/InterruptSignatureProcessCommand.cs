// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.InterruptSignatureProcess
{
    public class InterruptSignatureProcessCommand: IRequest<InterruptSignatureProcessCommandResponse>
    {
        public string IdSignProcessInstance { get; set; }

        public string InterruptionNote { get; set; }
    }

    public class InterruptSignatureProcessCommandResponse: MessageResponse
    {
	}
}