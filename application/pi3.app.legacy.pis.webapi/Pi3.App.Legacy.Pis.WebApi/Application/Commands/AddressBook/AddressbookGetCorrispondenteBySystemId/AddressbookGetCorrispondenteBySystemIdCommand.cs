// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
