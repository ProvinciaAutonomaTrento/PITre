// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetConfigurazioniRubricaComune
{
    public class GetConfigurazioniRubricaComuneCommand : IRequest<GetConfigurazioniRubricaComuneCommandResponse>
    {

    }

    public class GetConfigurazioniRubricaComuneCommandResponse
    {
        public ConfigurazioniRubricaComune Config { get; set; }
    }
}
