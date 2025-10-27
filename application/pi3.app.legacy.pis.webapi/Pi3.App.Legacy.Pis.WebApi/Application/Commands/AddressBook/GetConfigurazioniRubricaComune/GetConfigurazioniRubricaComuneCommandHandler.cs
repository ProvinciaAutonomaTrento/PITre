// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetConfigurazioniRubricaComune
{
    public class GetConfigurazioniRubricaComuneCommandHandler : IRequestHandler<GetConfigurazioniRubricaComuneCommand, GetConfigurazioniRubricaComuneCommandResponse>
    {
        public GetConfigurazioniRubricaComuneCommandHandler()
        {
            
        }

        public async Task<GetConfigurazioniRubricaComuneCommandResponse> Handle(GetConfigurazioniRubricaComuneCommand request, CancellationToken cancellationToken)
        {
            return new GetConfigurazioniRubricaComuneCommandResponse()
            {
                Config = new()
                {
                    GestioneAbilitata = true,
                    GestioneAmministrazioneAbilitata = true
                }
            };
        }
    }
}
