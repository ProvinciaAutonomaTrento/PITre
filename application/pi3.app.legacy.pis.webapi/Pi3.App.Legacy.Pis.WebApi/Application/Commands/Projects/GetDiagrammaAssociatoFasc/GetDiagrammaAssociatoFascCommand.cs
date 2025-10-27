// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetDiagrammaAssociatoFasc
{
    public class GetDiagrammaAssociatoFascCommand : IRequest<GetDiagrammaAssociatoFascCommandResponse>
    {
        public string IdTipoFasc { get; set; }
    }
    public class GetDiagrammaAssociatoFascCommandResponse
    {
        public int Output { get; set; }
    }
}
