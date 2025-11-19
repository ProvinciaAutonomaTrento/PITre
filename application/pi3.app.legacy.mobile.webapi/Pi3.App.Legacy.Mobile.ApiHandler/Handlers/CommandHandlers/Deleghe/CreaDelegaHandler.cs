// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;


namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.CommandHandlers.Deleghe;
public class CreaDelegaHandler( 
    ILogger<CreaDelegaHandler> logger,
    IMapper mapper,
    IDelegheService delegheService 
    ) : IRequestHandler<COMMAND.CreaDelegaCommand, RESULT.Deleghe.CreaDelegaResult>
{
    private readonly ILogger<CreaDelegaHandler> _logger = logger;
    private readonly IDelegheService _delegheService = delegheService;
    private readonly IMapper _mapper = mapper;

    public async Task<RESULT.Deleghe.CreaDelegaResult> Handle( COMMAND.CreaDelegaCommand request, CancellationToken cancellationToken )
    {
        this._logger.LogInformation("CreaDelegaHandler");
        SERVICE_REQUESTS.CreaDelegaRequest serviceRequest = this._mapper.Map<SERVICE_REQUESTS.CreaDelegaRequest>(request.Delega);
        bool serviceResult = await this._delegheService.CreaDelega(serviceRequest, cancellationToken);
        RESULT.Deleghe.CreaDelegaResult result = new()
        {
            Code = serviceResult ? RESULT.Deleghe.CreaDelegaResult.CreaDelegaResponseCode.OK : RESULT.Deleghe.CreaDelegaResult.CreaDelegaResponseCode.SYSTEM_ERROR
        };
        return result;
    }
}
