// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.QueryHandlers.Fascicoli;
public class GetFascicoloByIdHandler(
    IFascicoloService fascicoloService,
     IMapper mapper ) : IRequestHandler<QUERY.GetFascicoloInfoByIdQuery, RESPONSE.Fascicoli.GetFascicoloInfoByIdResponse>
{
    readonly IMapper _mapper = mapper;
    private readonly IFascicoloService _fascicoloService = fascicoloService;

    public async Task<RESPONSE.Fascicoli.GetFascicoloInfoByIdResponse> Handle( 
        QUERY.GetFascicoloInfoByIdQuery request, 
        CancellationToken cancellationToken )
    {
        SERVICE_DTO.Fascicolo fascicolo = await this._fascicoloService.GetFascicoloById(request.IdFascicolo, cancellationToken);
        RESPONSE.Fascicoli.GetFascicoloInfoByIdResponse response = new ()
        {
            FascInfo = this._mapper.Map<DTO.Fascicoli.Fascicolo>(fascicolo)
        };
        return response;
    }
}
