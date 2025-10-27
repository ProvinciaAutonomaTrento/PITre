// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteCompletoBySystemId
{
    public class AddressbookGetCorrispondenteCompletoBySystemIdCommand : IRequest<AddressbookGetCorrispondenteCompletoBySystemIdCommandResponse>
    {
        public string SystemId { get; set; }
        public string TipoIE { get; set; }
        public InfoUtente u { get; set; }
    }


    public class AddressbookGetCorrispondenteCompletoBySystemIdCommandResponse
    {
        public DocsPaVO.utente.Corrispondente output { get; set; }
    }
}
