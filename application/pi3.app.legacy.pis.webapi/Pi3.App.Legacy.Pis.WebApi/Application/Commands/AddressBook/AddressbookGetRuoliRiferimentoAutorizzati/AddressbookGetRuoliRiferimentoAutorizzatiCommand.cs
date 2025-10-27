// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetRuoliRiferimentoAutorizzati
{
    public class AddressbookGetRuoliRiferimentoAutorizzatiCommand : IRequest<AddressbookGetRuoliRiferimentoAutorizzatiCommandResponse>
    {
        public DocsPaVO.addressbook.QueryCorrispondenteAutorizzato Qca { get; set; }
        public DocsPaVO.utente.UnitaOrganizzativa Uo { get; set; }
    }


    public class AddressbookGetRuoliRiferimentoAutorizzatiCommandResponse
    {
        public Ruolo[] Output { get; set; }
    }
}
