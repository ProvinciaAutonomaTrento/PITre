// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetTypeRoleRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetTypeRole;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetTypeRole
{
    public class GetTypeRoleHandler : IRequestHandler<GetTypeRoleRequest, GetTypeRoleResult>
    {
        #region Public Members

        public GetTypeRoleHandler(ILogger<GetTypeRoleHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<GetTypeRoleResult> Handle(GetTypeRoleRequest request, CancellationToken cancellationToken)
        {
            var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            var tipoRuoloEntity = await this._dbContext.TipoRuoloEntities.AsNoTracking()
                .Where(t => t.ID_AMM == idTenantAsLong)
                .OrderBy(t => t.VAR_DESC_RUOLO)
                .ToListAsync();
            
            var output = _mapper.Map<TipoRuolo[]>(tipoRuoloEntity);

            return new GetTypeRoleResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetTypeRoleHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TipoRuoloEntity, TipoRuolo>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_RUOLO));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
