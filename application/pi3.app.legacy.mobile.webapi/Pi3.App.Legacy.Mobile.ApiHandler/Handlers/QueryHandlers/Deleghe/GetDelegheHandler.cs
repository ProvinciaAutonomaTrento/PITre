// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.QueryHandlers.Deleghe;
public class GetDelegheHandler(
    ILogger<GetDelegheHandler> logger,
    IMapper mapper,
    IDelegheService delegheService
    ) : IRequestHandler<QUERY.GetDelegheRequest, RESPONSE.Deleghe.GetDelegheResponse>
{
    private readonly ILogger<GetDelegheHandler> _logger = logger;
    private readonly IDelegheService _delegheService = delegheService;
    private readonly IMapper _mapper = mapper;

    public async Task<RESPONSE.Deleghe.GetDelegheResponse> Handle( QUERY.GetDelegheRequest request, CancellationToken cancellationToken )
    {
        this._logger.LogInformation("GetDelegheHandler");
        IEnumerable<SERVICE_DTO.Delega> deleghe = await this._delegheService.GetDeleghe(request.Stato, request.Tipo, cancellationToken);

        RESPONSE.Deleghe.GetDelegheResponse result = new()
        {
            Elements = this._mapper.Map<IEnumerable<DTO.Deleghe.Delega>>(deleghe),
            TotalRecordCount = deleghe.Count()
        };

        return result;
    }
}
