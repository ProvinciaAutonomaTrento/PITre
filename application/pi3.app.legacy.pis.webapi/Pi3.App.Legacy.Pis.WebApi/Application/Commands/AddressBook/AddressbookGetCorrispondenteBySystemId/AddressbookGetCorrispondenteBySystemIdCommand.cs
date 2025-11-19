// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.areaConservazione;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteBySystemId
{
    public class AddressbookGetCorrispondenteBySystemIdCommand : IRequest<AddressbookGetCorrispondenteBySystemIdCommandResponse>
    {
        public string SystemId { get; set; }
    }

    public class AddressbookGetCorrispondenteBySystemIdCommandResponse
    {
        public DocsPaVO.utente.Corrispondente Output{ get; set; }
    }
}
