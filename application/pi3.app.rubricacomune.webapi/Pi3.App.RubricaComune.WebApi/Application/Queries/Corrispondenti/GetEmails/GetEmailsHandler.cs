// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.RubricaComune.Infrastructure.EF.Entities;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;

namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.GetEmails
{
    public class GetEmailsHandler : IRequestHandler<GetEmailsRequest, GetEmailsResponse>
    {
        #region Public Members

        public GetEmailsHandler(ILogger<GetEmailsHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IRubricaComuneDbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this._mapper = this.InitializeMapper();
        }

        public async virtual Task<GetEmailsResponse> Handle(GetEmailsRequest request, CancellationToken cancellationToken)
        {
            if (!await this._dbContext.ElementiRubricaEntities.AnyAsync(e => e.ID == request.Id.AsLong(), cancellationToken: cancellationToken))
                throw new CorrispondenteNonTrovatoPi3Exception(request.Id);

            var emailEntities = await this._dbContext.EmailEntities.AsNoTracking().Where(e => e.IDELEMENTORUBRICA == request.Id.AsLong()).ToListAsync(cancellationToken: cancellationToken);

            return new GetEmailsResponse()
            {
                Emails = this._mapper.Map<IReadOnlyList<Email>>(emailEntities)
            };
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetEmailsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IRubricaComuneDbContext _dbContext;

        protected IMapper _mapper;

        protected IMapper InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EmailEntity, Queries.Corrispondenti.GetEmails.Email>()
                    .ForMember(dest => dest.Indirizzo, opt => opt.MapFrom(src => src.EMAIL))
                    .ForMember(dest => dest.Preferita, opt => opt.MapFrom(src => src.PREFERITA.HasValue && src.PREFERITA > 0))
                    .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.NOTE));
            });

           return configuration.CreateMapper();
        }

        #endregion
    }

}
