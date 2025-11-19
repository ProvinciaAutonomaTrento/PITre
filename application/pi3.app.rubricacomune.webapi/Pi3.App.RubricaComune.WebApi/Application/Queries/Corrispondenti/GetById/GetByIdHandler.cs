// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.RubricaComune.Infrastructure.EF.Entities;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;

namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.GetById
{
    public class GetByIdHandler : IRequestHandler<GetByIdRequest, GetByIdResponse>
    {
        #region Public Members

        public GetByIdHandler(ILogger<GetByIdHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IRubricaComuneDbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this._mapper = this.InitializeMapper();
        }

        public async Task<GetByIdResponse> Handle(GetByIdRequest request, CancellationToken cancellationToken)
        {
            if (!await this._dbContext.ElementiRubricaEntities.AnyAsync(e => e.ID == request.Id.AsLong(), cancellationToken: cancellationToken))
                throw new CorrispondenteNonTrovatoPi3Exception(request.Id);

            var entity = await this._dbContext.ElementiRubricaEntities.AsNoTracking().FirstAsync(e => e.ID == request.Id.AsLong(), cancellationToken: cancellationToken);

            return new GetByIdResponse()
            {
                Corrispondente = this._mapper.Map<Corrispondente>(entity)
            };
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetByIdHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IRubricaComuneDbContext _dbContext;

        protected IMapper _mapper;

        protected IMapper InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                Corrispondente.CreateMapFromElementoRubricaEntity(cfg);
            });

            return configuration.CreateMapper();
        }

        #endregion
    }

}
