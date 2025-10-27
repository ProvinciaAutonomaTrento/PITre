// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Handlers.CambiaFascicolazionePrimaria;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetCapComuniRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetCapComuni;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetCapComuni
{
    public class GetCapComuniHandler : IRequestHandler<GetCapComuniRequest, GetCapComuniResult>
    {
        protected readonly ILogger<GetCapComuniHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected IMapper _mapper = null;

        public GetCapComuniHandler(
            ILogger<GetCapComuniHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IPi3DbContext dbContext,
            IConfigurationService configurationService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
            this.InitializeMapper();
        }

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<InfoComuniEntity, InfoComune>()
                .ForMember(dest => dest.CAP, opt => opt.MapFrom(src => src.VAR_CAP))
                .ForMember(dest => dest.COMUNE, opt => opt.MapFrom(src => src.VAR_COMUNE))
                .ForMember(dest => dest.PROVINCIA, opt => opt.MapFrom(src => src.VAR_PROVINCIA));

            });
            this._mapper = configuration.CreateMapper();
        }


        public async Task<GetCapComuniResult> Handle(GetCapComuniRequest request, CancellationToken cancellationToken)
        {
            InfoComune output = new InfoComune();

            try
            {
                this._logger.LogDebug("GetCapComuni");
                var infoComEnt = await this._dbContext.InfoComuniEntities.AsNoTracking().FirstOrDefaultAsync(ic => ic.VAR_CAP == request.cap && 
                (string.IsNullOrEmpty(request.comune) ? true : ic.VAR_COMUNE != null ? ic.VAR_COMUNE.ToUpper() == request.comune.Replace("'", "''").ToUpper() : string.IsNullOrEmpty(request.comune) ) );
            
                if( infoComEnt != null )
                {
                    output = this._mapper.Map<InfoComune>(infoComEnt);
                }


            }catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new GetCapComuniResult(output);
        }

    }
}
