// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.DiagrammaStato;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetDiagrammaById
{
    public class GetDiagrammaByIdCommand : IRequest<GetDiagrammaByIdCommandResponse>
    {
        public string IdDiagramma{ get; set; }
    }

    public class GetDiagrammaByIdCommandResponse
    {
        public DiagrammaStato Output { get; set; }
    }
}
