// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

using Pi3.App.Legacy.Mobile.ApiHandler.Handlers.QueryHandlers.Users;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
using Pi3.Core.Services.Principal;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.QueryHandlers.Documents;

public class GetDocumentInfoHandler( 
    ILogger<GetUserHandler> logger,
    IMapper mapper,
    IClaimsPrincipalService claimsPrincipalService,
    IDocumentService documentService,
    IConfigurationService configurationService
    ) : IRequestHandler<QUERY.GetDocumentInfoQuery, DTO.Documents.GetDocumentInfoDto>
{
    readonly ILogger<GetUserHandler> _logger = logger;
    readonly IMapper _mapper = mapper;
    readonly IClaimsPrincipalService _claimsPrincipalService = claimsPrincipalService;
    readonly IDocumentService _documentService = documentService;
    readonly IConfigurationService _configurationService = configurationService;

    public async Task<DTO.Documents.GetDocumentInfoDto> Handle(QUERY.GetDocumentInfoQuery request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Handle GetDocumentInfo");

        long? idAmministrazione = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

        long? idRuoloPubblico = null;
        if ( idAmministrazione.HasValue )
        {
            string? idRuoloPubblicoDB = await this._configurationService.GetChiaveByIdAmmAsync("ENABLE_FASCICOLO_PUBBLICO", idAmministrazione.Value, cancellationToken);
            idRuoloPubblico = long.TryParse(idRuoloPubblicoDB, out long tempLongValue) ? tempLongValue : null ;
        }

        SERVICE_REQUESTS.GetDocumentRequest getDocumentRequest = new()
        {
            IdDocumento = request.IdDocument,
            IdRuoloPubblico = idRuoloPubblico
        };

        SERVICE_DTO.Documento result 
                = await this._documentService
                    .GetDocumentAsync(getDocumentRequest, cancellationToken);

        Models.Documento documentoPrincipale = this._mapper.Map<Models.Documento>(result);
        
        DTO.Documents.GetDocumentInfoDto output = new() { 
            DocInfo = documentoPrincipale,
            Allegati = null 
        };

        if ( !result.IdDocPrincipale.HasValue )
        {
            this._logger.LogDebug("Recupero gli allegati");
            IEnumerable<SERVICE_DTO.Documento> allegati = await this._documentService.GetDocumentAttachmentsAsync(getDocumentRequest, cancellationToken);
            output.Allegati = this._mapper.Map<IEnumerable<Models.Documento>>(allegati);
        }

        return output;
    }

}
