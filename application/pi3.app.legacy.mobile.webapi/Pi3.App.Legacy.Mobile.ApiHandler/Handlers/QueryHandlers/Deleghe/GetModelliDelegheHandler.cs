// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.QueryHandlers.Deleghe;
public class GetModelliDelegheHandler(
    ILogger<GetModelliDelegheHandler> logger,
    IMapper mapper,
    IDelegheService delegheService 
    ) : IRequestHandler<QUERY.GetModelliDelegheRequest, RESPONSE.Deleghe.GetModelliDelegheResponse>
{
    private readonly ILogger<GetModelliDelegheHandler> _logger = logger;
    private readonly IDelegheService _delegheService = delegheService;
    private readonly IMapper _mapper = mapper;

    public Task<RESPONSE.Deleghe.GetModelliDelegheResponse> Handle( QUERY.GetModelliDelegheRequest request, CancellationToken cancellationToken )
    {
        throw new NotImplementedException();
    }
}
