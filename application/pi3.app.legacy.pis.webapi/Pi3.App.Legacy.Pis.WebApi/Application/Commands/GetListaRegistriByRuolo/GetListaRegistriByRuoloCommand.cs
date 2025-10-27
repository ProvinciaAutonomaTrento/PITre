// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.GetListaRegistriByRuolo
{
    public class GetListaRegistriByRuoloCommand : IRequest<GetListaRegistriByRuoloCommandResponse>
    {
        public string IdRuolo { get; set; }
    }
    public class GetListaRegistriByRuoloCommandResponse
    {
        public DocsPaVO.utente.Registro[] output { get; set; }
        public GetListaRegistriByRuoloCommandResponse()
        {

        }
        public GetListaRegistriByRuoloCommandResponse(DocsPaVO.utente.Registro[] _output)
        {
            this.output = _output;
        }
    }
}
