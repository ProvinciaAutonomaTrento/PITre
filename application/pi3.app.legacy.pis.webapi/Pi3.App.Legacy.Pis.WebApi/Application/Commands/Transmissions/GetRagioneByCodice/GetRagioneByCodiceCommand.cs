// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetRagioneByCodice
{
    public class GetRagioneByCodiceCommand : IRequest<GetRagioneByCodiceCommandResponse>
    {
        public string IdAmm { get; set; }
        public string Codice { get; set; }
    }
    public class GetRagioneByCodiceCommandResponse
    {
        public DocsPaVO.trasmissione.RagioneTrasmissione? Output{ get; set; }
    }
}
