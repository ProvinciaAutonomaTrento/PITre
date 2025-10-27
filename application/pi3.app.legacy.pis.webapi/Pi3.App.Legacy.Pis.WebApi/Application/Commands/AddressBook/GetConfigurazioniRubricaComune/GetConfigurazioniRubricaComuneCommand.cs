// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
