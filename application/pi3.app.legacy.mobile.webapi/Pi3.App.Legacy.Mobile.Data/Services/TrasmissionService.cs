// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;

using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;
using SELECT_TEMPLATES = Pi3.App.Legacy.Mobile.Models.SelectTemplates;
using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;
using Pi3.Core.Services.Principal;


namespace Pi3.App.Legacy.Mobile.Data.Services;

public class TrasmissionService(
    IMapper mapper,
    IClaimsPrincipalService claimsPrincipalService,
    IConfigurationRepository configurationRepository,
    ITrasmissionRepository trasmissionRepository,
    IPeopleRepository peopleRepository) : ITrasmissionService
{
    IClaimsPrincipalService _claimsPrincipalService = claimsPrincipalService;
    readonly IConfigurationRepository _configurationRepository = configurationRepository;
    readonly IMapper _mapper = mapper;
    readonly ITrasmissionRepository _trasmissionRepository = trasmissionRepository;
    readonly IPeopleRepository _peopleRepository = peopleRepository;

    public async Task<SERVICE_DTO.Trasmissione> GetDettagliTrasmissioneUtenteAsync( 
        long id, 
        CancellationToken cancellationToken = default )
    {
        long idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);

        SERVICE_DTO.Trasmissione trasmissione;
        SELECT_TEMPLATES.TrasmissioneJoinSingolaRagioneUtente? trasmissioneDB
            = await this._trasmissionRepository.Get_TrasmissioneSignola_By_IdTrasmissioneBy_IdUtente_Async(id, idPeople, cancellationToken);
        trasmissione = this._mapper.Map<SERVICE_DTO.Trasmissione>(trasmissioneDB )
            ?? throw new EXCEPTIONS.TrasmissionNotFoundException(id);

        SELECT_TEMPLATES.Utente? utenteTrasmissione;
        SELECT_TEMPLATES.Utente? utenteDelegato;
        if ( trasmissioneDB?.IdPeople.HasValue ?? throw new EXCEPTIONS.UnexpectedException("Utente trasmissione non definito") )
        {
            utenteTrasmissione = await this._peopleRepository.GetUtenteByIdAsync(trasmissioneDB.IdPeople.Value, cancellationToken)
                ?? throw new EXCEPTIONS.UnexpectedException("Utente trasmissione non recuperato");
            if ( trasmissioneDB?.IdPeopleDelegato.HasValue ?? false )
            {
                utenteDelegato = await this._peopleRepository.GetUtenteByIdAsync(trasmissioneDB.IdPeopleDelegato.Value, cancellationToken)
                    ?? throw new EXCEPTIONS.UnexpectedException("Utente trasmissione non recuperato");
                trasmissione.Mittente = String.Format("{0} {1} delegato da {2} {3}", 
                                                        utenteDelegato.Cognome, 
                                                        utenteDelegato.Nome, 
                                                        utenteTrasmissione.Cognome, 
                                                        utenteTrasmissione.Nome);
            }
            else
            {
                trasmissione.Mittente = String.Format("{0} {1}", utenteTrasmissione.Cognome, utenteTrasmissione.Nome);
            }
        }

        return trasmissione;
    }
}
