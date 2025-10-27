// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.AddDocInProject
{
    public class AddDocInProjectCommand: IRequest<AddDocInProjectCommandResponse>
    {
        public string IdDocument { get; set; }
        public string IdProject { get; set; }
        public string CodeProject { get; set; }
    }

    public class AddDocInProjectCommandResponse: MessageResponse
    {

	}
}