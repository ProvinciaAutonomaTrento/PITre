// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Pi3.App.Legacy.Mobile.Data.Services;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.QueryHandlers.Trasmissions;
public class GetTrasmissionByIdHandler(
    ITrasmissionService trasmissionService,
    IMapper mapper ) : IRequestHandler<QUERY.GetTrasmissionById, RESPONSE.GetTrasmissioneByIdResponse>
{
    readonly IMapper _mapper = mapper;
    readonly ITrasmissionService _trasmissionService = trasmissionService;

    public async Task<RESPONSE.GetTrasmissioneByIdResponse> Handle( QUERY.GetTrasmissionById request, CancellationToken cancellationToken )
    {
        SERVICE_DTO.Trasmissione trasmissione = await this._trasmissionService.GetDettagliTrasmissioneUtenteAsync(request.IdTrasmissione, cancellationToken);
        RESPONSE.GetTrasmissioneByIdResponse response = new()
        {
            Trasmissione = this._mapper.Map<DTO.Trasmissione>( trasmissione )
        };


        return response;
    }
}
