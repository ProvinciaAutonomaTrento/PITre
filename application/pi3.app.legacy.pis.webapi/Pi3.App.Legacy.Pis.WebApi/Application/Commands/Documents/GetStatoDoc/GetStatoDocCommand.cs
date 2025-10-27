// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DiagrammaStato;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetStatoDoc
{
    public class GetStatoDocCommand : IRequest<GetStatoDocCommandResponse>
    {
        public string DocNumber { get; set; }
    }

    public class GetStatoDocCommandResponse
    {
        public Stato Output { get; set; }
    }
}
