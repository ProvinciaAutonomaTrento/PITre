// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookInsertCorrispondente
{
    public class AddressbookInsertCorrispondenteCommand : IRequest<AddressbookInsertCorrispondenteCommandResponse>
    {
        public DocsPaVO.utente.Corrispondente Corrispondente { get; set; }
        public DocsPaVO.utente.Corrispondente Parent { get; set; }
        public DocsPaVO.utente.InfoUtente Iu { get; set; }
    }

    public class AddressbookInsertCorrispondenteCommandResponse
    {
        public DocsPaVO.utente.Corrispondente Output { get; set; }
    }
}
