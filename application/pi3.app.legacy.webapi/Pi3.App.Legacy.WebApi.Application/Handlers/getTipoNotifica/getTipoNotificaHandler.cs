// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.DatiCert;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getTipoNotificaRequest = Pi3.App.Legacy.WebApi.Application.Requests.getTipoNotifica;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getTipoNotifica
{
    public class getTipoNotificaHandler : IRequestHandler<getTipoNotificaRequest, getTipoNotificaResult>
    {
        #region Public Members

        public getTipoNotificaHandler(ILogger<getTipoNotificaHandler> logger, 
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

        public async Task<getTipoNotificaResult> Handle(getTipoNotificaRequest request, CancellationToken cancellationToken)
        {
            TipoNotifica output = null;

            var tipoNotificaEntity = await _dbContext.TipoNotificaEntities.AsNoTracking()
                .FirstOrDefaultAsync(t => t.SYSTEM_ID == request.systemIdTipoNotifica.AsLong());

            output = _mapper.Map<TipoNotifica>(tipoNotificaEntity);

            return new getTipoNotificaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getTipoNotificaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TipoNotificaEntity, TipoNotifica>()
                     .ForMember(dest => dest.idTipoNotifica, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.codiceNotifica, opt => opt.MapFrom(src => src.VAR_CODICE_NOTIFICA))
                     .ForMember(dest => dest.descrizioneNotifica, opt => opt.MapFrom(src => src.VAR_DESCRIZIONE));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }
}