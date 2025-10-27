// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.Smistamento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getSistemaEsternoByCodeAppRequest = Pi3.App.Legacy.WebApi.Application.Requests.getSistemaEsternoByCodeApp;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getSistemaEsternoByCodeApp
{
    public class getSistemaEsternoByCodeAppHandler : IRequestHandler<getSistemaEsternoByCodeAppRequest, getSistemaEsternoByCodeAppResult>
    {
        #region Public Members

        public getSistemaEsternoByCodeAppHandler(ILogger<getSistemaEsternoByCodeAppHandler> logger,
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

        public async Task<getSistemaEsternoByCodeAppResult> Handle(getSistemaEsternoByCodeAppRequest request, CancellationToken cancellationToken)
        {
            SistemaEsterno output = null!;

            try
            {
                if (request.codeApp != null)
                {
                    long idAmministrazione = Convert.ToInt64(request.idAmm);
                    var entity = await this._dbContext.ExternalSystemEntities
                        .Where(e => e.ID_AMM == idAmministrazione && e.VAR_CODE_APPLICATION.ToUpper() == request.codeApp.ToUpper())
                        .FirstOrDefaultAsync();

                    if (entity != null)
                        output = this._mapper.Map<SistemaEsterno>(entity);
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new getSistemaEsternoByCodeAppResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getSistemaEsternoByCodeAppHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ExternalSystemEntity, SistemaEsterno>()
                    .ForMember(dest => dest.IdSistemaEsterno, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.idAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.CodiceApplicazione, src => src.MapFrom(opt => opt.VAR_CODE_APPLICATION))
                    .ForMember(dest => dest.DescEstesa, src => src.MapFrom(opt => opt.VAR_DESC_ESTESA))
                    .ForMember(dest => dest.Diritti, src => src.MapFrom(opt => opt.VAR_PIS_METHODS_ALLOWED))
                    .ForMember(dest => dest.idRuoloAssociato, src => src.MapFrom(opt => opt.ID_SYSTEM_ROLE))
                    .ForMember(dest => dest.UserIdAssociato, src => src.MapFrom(opt => opt.VAR_USER_ID))
                    .ForMember(dest => dest.TokenPeriod, src => src.MapFrom(opt => opt.VAR_TKN_TIME));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
