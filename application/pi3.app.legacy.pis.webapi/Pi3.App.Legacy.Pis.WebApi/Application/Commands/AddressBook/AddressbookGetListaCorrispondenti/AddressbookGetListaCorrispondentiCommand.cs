// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.addressbook;
using MediatR;
using DocsPaVO.utente;
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetListaCorrispondenti
{
    public class AddressbookGetListaCorrispondentiCommand : IRequest<AddressbookGetListaCorrispondentiCommandResponse>
    {
        public QueryCorrispondente QueryCorrispondente { get; set; }
    }
    public class AddressbookGetListaCorrispondentiCommandResponse
    {
        public Corrispondente[] Output { get; set; }
    }
}
