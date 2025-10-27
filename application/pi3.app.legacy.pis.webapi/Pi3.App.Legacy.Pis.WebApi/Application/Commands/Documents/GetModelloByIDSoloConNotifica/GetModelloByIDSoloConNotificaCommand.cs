// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetModelloByIDSoloConNotifica
{
    public class GetModelloByIDSoloConNotificaCommand : IRequest<GetModelloByIDSoloConNotificaCommandResponse>
    {
        public string IdAmm { get; set; }
        public string IdModello { get; set; }
    }
    public class GetModelloByIDSoloConNotificaCommandResponse
    {
        public DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione Output{ get; set; }
    }
}
