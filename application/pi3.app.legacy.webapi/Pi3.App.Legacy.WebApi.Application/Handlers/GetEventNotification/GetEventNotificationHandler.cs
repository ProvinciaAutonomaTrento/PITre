// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.LibroFirma;
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
using GetEventNotificationRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetEventNotification;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetEventNotification
{
    public class GetEventNotificationHandler : IRequestHandler<GetEventNotificationRequest, GetEventNotificationResult>
    {
        #region Public Members

        public GetEventNotificationHandler(ILogger<GetEventNotificationHandler> logger, 
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

        public async Task<GetEventNotificationResult> Handle(GetEventNotificationRequest request, CancellationToken cancellationToken)
        {
            var anagraficaEventiEntity = await this._dbContext.AnagraficaEventiEntities.
               AsNoTracking()
               .Where(a => a.CHA_TIPO_EVENTO == "N")
               .GroupBy(a => new { a.GRUPPO, a.DESCRIZIONE })
               .Select(a => new AnagraficaEventiEntity()
               {
                   GRUPPO = a.Key.GRUPPO,
                   DESCRIZIONE = a.Key.DESCRIZIONE
               })
               .ToListAsync();

            var output = _mapper.Map<AnagraficaEventi[]>(anagraficaEventiEntity);

            return new GetEventNotificationResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetEventNotificationHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AnagraficaEventiEntity, AnagraficaEventi>()
                    .ForMember(dest => dest.gruppo, src => src.MapFrom(opt => opt.GRUPPO))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.DESCRIZIONE));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
