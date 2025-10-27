// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.RemoveDocument
{
    public class RemoveDocumentCommand: IRequest<RemoveDocumentCommandResponse>
    {
        public string IdDocument
        {
            get;
            set;
        }


        public string RemovalNote
        {
            get;
            set;
        }
    }

    public class RemoveDocumentCommandResponse: MessageResponse
    {
	}
}