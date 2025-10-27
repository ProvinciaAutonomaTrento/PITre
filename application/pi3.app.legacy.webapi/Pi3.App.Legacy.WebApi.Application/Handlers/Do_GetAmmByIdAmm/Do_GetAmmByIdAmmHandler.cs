// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
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
using Do_GetAmmByIdAmmRequest = Pi3.App.Legacy.WebApi.Application.Requests.Do_GetAmmByIdAmm;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.Do_GetAmmByIdAmm
{
    public class Do_GetAmmByIdAmmHandler : IRequestHandler<Do_GetAmmByIdAmmRequest, Do_GetAmmByIdAmmResult>
    {
        #region Public Members

        public Do_GetAmmByIdAmmHandler(ILogger<Do_GetAmmByIdAmmHandler> logger, 
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

        public async Task<Do_GetAmmByIdAmmResult> Handle(Do_GetAmmByIdAmmRequest request, CancellationToken cancellationToken)
        {
            var instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, true);
            var idTenantAsLong = Convert.ToInt64(request.idAmm);

            var amministraEntities = await this._distributedCache.FromCache(instance!, this._dbContext.AmministraEntities);
            var amministraEntity = amministraEntities.Where(a => a.SYSTEM_ID == idTenantAsLong).FirstOrDefault();

            var output = _mapper.Map<PR_Amministrazione>(amministraEntity);

            return new Do_GetAmmByIdAmmResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<Do_GetAmmByIdAmmHandler> _logger;
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
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.VAR_DESC_AMM));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
