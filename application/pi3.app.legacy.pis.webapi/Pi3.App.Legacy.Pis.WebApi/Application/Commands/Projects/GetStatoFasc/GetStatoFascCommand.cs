// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetStatoFasc
{
    public class GetStatoFascCommand : IRequest<GetStatoFascCommandResponse>
    {
        public string IdProject { get; set; }
    }

    public class GetStatoFascCommandResponse
    {
        public DocsPaVO.DiagrammaStato.Stato Output{ get; set; }
    }
}
