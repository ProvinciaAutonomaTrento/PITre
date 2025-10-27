// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetRuolo
{
    public class GetRuoloCommand : IRequest<GetRuoloCommandResponse>
    {
        public string IdCorrGlobali { get; set; }
    }

    public class GetRuoloCommandResponse
    {
        public DocsPaVO.utente.Ruolo Output { get; set; }
    }
}
