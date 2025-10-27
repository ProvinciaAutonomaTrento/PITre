// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;

using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.QueryHandlers.Trasmissions;

public class GetUserTrasmissionByIdIdPeopleHandler(
    ILogger<GetUserTrasmissionByIdIdPeopleHandler> logger,
    IMapper mapper,
    ITrasmissionService trasmissionService
    ) : IRequestHandler<QUERY.GetUserTrasmissionById, DTO.Trasmissions.GetTrasmissionByIdDTO>
{
    readonly ILogger<GetUserTrasmissionByIdIdPeopleHandler> _logger = logger;
    readonly IMapper _mapper = mapper;
    readonly ITrasmissionService _trasmissionService = trasmissionService;

    public async Task<DTO.Trasmissions.GetTrasmissionByIdDTO> Handle( QUERY.GetUserTrasmissionById request, CancellationToken cancellationToken )
    {
        this._logger.LogInformation("Handle GetTrasmissionById");

        SERVICE_DTO.Trasmissione trasmissioneDB 
            = await this._trasmissionService.GetDettagliTrasmissioneUtenteAsync(
                request.IdTrasmissione, 
                cancellationToken)
                    ?? throw new EXCEPTIONS.TrasmissionNotFoundException(request.IdTrasmissione);

        DTO.Trasmissions.GetTrasmissionByIdDTO result = this._mapper.Map<DTO.Trasmissions.GetTrasmissionByIdDTO>(trasmissioneDB);

        return result;
    }

}
