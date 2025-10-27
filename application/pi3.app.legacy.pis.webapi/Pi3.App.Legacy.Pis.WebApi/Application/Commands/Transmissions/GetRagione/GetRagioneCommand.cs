// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetRagione
{
    public class GetRagioneCommand : IRequest<GetRagioneCommandResponse>
    {
        public string TipoDest { get; set; }
        public string IdAmm { get; set; }
    }
    public class GetRagioneCommandResponse
    {
        public DocsPaVO.trasmissione.RagioneTrasmissione Output { get; set; }
    }
}
