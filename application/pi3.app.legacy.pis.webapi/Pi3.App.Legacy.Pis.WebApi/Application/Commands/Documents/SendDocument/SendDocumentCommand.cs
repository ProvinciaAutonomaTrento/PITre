// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SendDocument
{
    public class SendDocumentCommand: IRequest<SendDocumentCommandResponse>
    {
        public string IdDocument { get; set; }
        public string Signature { get; set; }
    }

    public class SendDocumentCommandResponse:MessageResponse
    {
	}
}