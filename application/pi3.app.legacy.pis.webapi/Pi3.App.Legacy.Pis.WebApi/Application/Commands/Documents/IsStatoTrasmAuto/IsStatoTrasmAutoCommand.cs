// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using DocsPaVO.Modelli_Trasmissioni;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.IsStatoTrasmAuto
{

    public class IsStatoTrasmAutoCommand : IRequest<IsStatoTrasmAutoCommandResponse>
    {
        public string IdAmm { get; set; }
        public string IdStato { get; set; }
        public string IdTemplate { get; set; }
    }

    public class IsStatoTrasmAutoCommandResponse
    {
        public ModelloTrasmissione[] Output { get; set; }
    }
}
