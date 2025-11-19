// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using DocsPaVO.Modelli_Trasmissioni;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetModelloByID
{
    public class GetModelloByIDCommand : IRequest<GetModelloByIDCommandResponse>
    {
        public string IdAmm { get; set; }
        public string IdModello { get; set; }
    }

    public class GetModelloByIDCommandResponse
    {
        public ModelloTrasmissione Output { get; set; }
    }
}
