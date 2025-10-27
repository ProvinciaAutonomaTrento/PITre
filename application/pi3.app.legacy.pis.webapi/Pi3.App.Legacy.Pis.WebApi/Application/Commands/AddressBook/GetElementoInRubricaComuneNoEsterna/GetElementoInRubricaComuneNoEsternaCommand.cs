// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Spedizione;
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetElementoInRubricaComuneNoEsterna
{
    public class GetElementoInRubricaComuneNoEsternaCommand : IRequest<GetElementoInRubricaComuneNoEsternaCommandResponse>
    {
        public string Codice { get; set; }
        public InfoUtente InfoUtente { get; set; }
    }

    public class GetElementoInRubricaComuneNoEsternaCommandResponse
    {
        public Corrispondente Corrispondente { get; set; }

    }
}
