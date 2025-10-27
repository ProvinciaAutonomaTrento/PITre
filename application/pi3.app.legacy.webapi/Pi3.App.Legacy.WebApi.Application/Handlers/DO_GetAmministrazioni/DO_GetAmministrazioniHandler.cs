// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.ProspettiRiepilogativi;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO_GetAmministrazioniRequest = Pi3.App.Legacy.WebApi.Application.Requests.DO_GetAmministrazioni;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DO_GetAmministrazioni
{

    public class DO_GetAmministrazioniHandler : IRequestHandler<DO_GetAmministrazioniRequest, DO_GetAmministrazioniResult>
    {
        #region Public Members

        public DO_GetAmministrazioniHandler(ILogger<DO_GetAmministrazioniHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext,
            IDistributedCache distributedCache)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._distributedCache = distributedCache;
            this._dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<DO_GetAmministrazioniResult> Handle(DO_GetAmministrazioniRequest request, CancellationToken cancellationToken)
        {
            var instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, true);
            
            var amministraEntities = await this._distributedCache.FromCache(instance!, this._dbContext.AmministraEntities);

            var output = _mapper.Map<PR_Amministrazione[]>(amministraEntities);

            return new DO_GetAmministrazioniResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DO_GetAmministrazioniHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AmministrazioneEntity, PR_Amministrazione>()
                    .ForMember(dest => dest.System_id, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.Codice, src => src.MapFrom(opt => opt.VAR_CODICE_AMM))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.VAR_DESC_AMM))
                    .ForMember(dest => dest.Libreria, src => src.MapFrom(opt => opt.VAR_LIBRERIA));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
