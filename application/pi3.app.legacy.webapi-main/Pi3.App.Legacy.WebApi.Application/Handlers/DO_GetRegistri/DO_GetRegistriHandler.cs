// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.ProspettiRiepilogativi;
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
using DO_GetRegistriRequest = Pi3.App.Legacy.WebApi.Application.Requests.DO_GetRegistri;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DO_GetRegistri
{
    public class DO_GetRegistriHandler : IRequestHandler<DO_GetRegistriRequest, DO_GetRegistriResult>
    {
        #region Public Members

        public DO_GetRegistriHandler(ILogger<DO_GetRegistriHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator, 
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<DO_GetRegistriResult> Handle(DO_GetRegistriRequest request, CancellationToken cancellationToken)
        {
            var idAmmAsLong = Convert.ToInt64(request.idAmm);

            var registroEntity = await this._dbContext.RegistroEntities.AsNoTracking()
                .Where(r => r.ID_AMM == idAmmAsLong)
                .Select(r => new RegistroEntity
                {
                    SYSTEM_ID = r.SYSTEM_ID,
                    VAR_CODICE = r.VAR_CODICE,
                    VAR_DESC_REGISTRO = r.VAR_DESC_REGISTRO
                })
                .ToListAsync();

            var output = _mapper.Map<RegistroEntity[]>(registroEntity);

            return new DO_GetRegistriResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DO_GetRegistriHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegistroEntity, PR_Registro>()
                    .ForMember(dest => dest.System_id, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.Codice, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.VAR_DESC_REGISTRO));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
