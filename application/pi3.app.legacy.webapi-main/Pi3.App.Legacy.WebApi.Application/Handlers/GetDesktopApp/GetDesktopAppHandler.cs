// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.DesktopApps;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetDesktopAppRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetDesktopApp;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetDesktopApp
{
    public class GetDesktopAppHandler : IRequestHandler<GetDesktopAppRequest, GetDesktopAppResult>
    {
        #region Public Members

        public GetDesktopAppHandler(ILogger<GetDesktopAppHandler> logger,
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

        public async Task<GetDesktopAppResult> Handle(GetDesktopAppRequest request, CancellationToken cancellationToken)
        {
            DesktopApp output = null;

            try
            {
                var desktopAppsEntity = await this._dbContext.DesktopAppEntities.AsNoTracking().FirstOrDefaultAsync(a => a.NOME == request.AppName);

                if (desktopAppsEntity != null)
                    output = this._mapper.Map<DesktopApp>(desktopAppsEntity);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new GetDesktopAppResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDesktopAppHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DesktopAppEntity, DesktopApp>()
                     .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.NOME))
                     .ForMember(dest => dest.Versione, opt => opt.MapFrom(src => src.VERSIONE))
                     .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.PATH))
                     .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.DESCRIZIONE));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
