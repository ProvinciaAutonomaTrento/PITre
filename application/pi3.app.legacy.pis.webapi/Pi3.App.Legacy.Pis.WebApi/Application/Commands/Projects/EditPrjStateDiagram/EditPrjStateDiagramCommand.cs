// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.EditPrjStateDiagram
{
    public class EditPrjStateDiagramCommand: IRequest<EditPrjStateDiagramCommandResponse>
    {
        public string StateOfDiagram { get; set; }
        public string IdProject { get; set; }
        public string ClassificationSchemeId { get; set; }
        public string CodeProject { get; set; }
    }

    public class EditPrjStateDiagramCommandResponse: MessageResponse
    {
	}
}