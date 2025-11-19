// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.amministrazione;
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
using getInfoDisservizioRequest = Pi3.App.Legacy.WebApi.Application.Requests.getInfoDisservizio;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getInfoDisservizio
{
    public class getInfoDisservizioHandler : IRequestHandler<getInfoDisservizioRequest, getInfoDisservizioResult>
    {
        #region Public Members

        public getInfoDisservizioHandler(ILogger<getInfoDisservizioHandler> logger, 
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

        public async Task<getInfoDisservizioResult> Handle(getInfoDisservizioRequest request, CancellationToken cancellationToken)
        {
            Disservizio output = new Disservizio();

            try
            {
                var disservizioEntity = await this._dbContext.DisservizioEntities.AsNoTracking().FirstOrDefaultAsync();
                
                if(disservizioEntity != null)
                    output = this._mapper.Map<Disservizio>(disservizioEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new getInfoDisservizioResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getInfoDisservizioHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DisservizioEntity, Disservizio>()
                     .ForMember(dest => dest.system_id, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.stato, opt => opt.MapFrom(src => src.STATO))
                     .ForMember(dest => dest.testo_notifica, opt => opt.MapFrom(src => src.TESTO_NOTIFICA))
                     .ForMember(dest => dest.testo_email_notifica, opt => opt.MapFrom(src => src.TESTO_EMAIL_NOTIFICA))
                     .ForMember(dest => dest.testo_cortesia, opt => opt.MapFrom(src => src.TESTO_PAG_CORTESIA))
                     .ForMember(dest => dest.testo_email_ripresa, opt => opt.MapFrom(src => src.TESTO_EMAIL_RIPRESA))
                     .ForMember(dest => dest.notificato, opt => opt.MapFrom(src => src.NOTIFICATO));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
