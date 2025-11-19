// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetRuoloRespUoFromUo
{
    public class AddressbookGetRuoloRespUoFromUoCommand : IRequest<AddressbookGetRuoloRespUoFromUoCommandResponse>
    {
        public string IdCorrGlobaliUo { get; set; }
        public string TipoRuolo { get; set; }
        public string IdCorr { get; set; }
    }

    public class AddressbookGetRuoloRespUoFromUoCommandResponse
    {
        public string Output{ get; set; }
    }
}
