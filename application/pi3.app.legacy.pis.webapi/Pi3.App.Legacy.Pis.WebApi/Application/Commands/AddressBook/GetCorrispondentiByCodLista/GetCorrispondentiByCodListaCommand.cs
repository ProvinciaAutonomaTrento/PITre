// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;
using System.Diagnostics;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondentiByCodLista
{
    public class GetCorrispondentiByCodListaCommand : IRequest<GetCorrispondentiByCodListaCommandResponse>
    {
        public string CodiceLista { get; set; }
        public InfoUtente InfoUtente { get; set; }
        
    }


    public class GetCorrispondentiByCodListaCommandResponse
    {
        public List<Corrispondente> Corrispondenti{ get; set; }
    }
}
