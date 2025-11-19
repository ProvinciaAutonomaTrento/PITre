// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.Interoperabilita.Semplificata;
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

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.LoadSimplifiedInteroperabilitySettings
{
    public class LoadSimplifiedInteroperabilitySettingsHandler : IRequestHandler<Application.Requests.LoadSimplifiedInteroperabilitySettings, LoadSimplifiedInteroperabilitySettingsResult>
    {
        #region Public Members

        public LoadSimplifiedInteroperabilitySettingsHandler(ILogger<LoadSimplifiedInteroperabilitySettingsHandler> logger, 
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

        public async Task<LoadSimplifiedInteroperabilitySettingsResult> Handle(Application.Requests.LoadSimplifiedInteroperabilitySettings request, CancellationToken cancellationToken)
        {
            InteroperabilitySettings output = null;

            try 
            {
                if (string.IsNullOrEmpty(request.registryId))
                {
                    return new LoadSimplifiedInteroperabilitySettingsResult(new InteroperabilitySettings()
                    {
                        IsEnabledInteroperability = false,
                        RegistryId = request.registryId,
                        RoleId = 0,
                        UserId = 0,
                        KeepPrivate = false,
                        ManagementMode = ManagementType.M
                    });
                }

                long idRegistro = request.registryId.AsLong();
                var interoperabilitySettingsEntity = await this._dbContext.InteroperabilitySettingEntities.FirstOrDefaultAsync(s => s.REGISTRYID == idRegistro);
                if(interoperabilitySettingsEntity != null)
                    output = this._mapper.Map<InteroperabilitySettings>(interoperabilitySettingsEntity);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new LoadSimplifiedInteroperabilitySettingsResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<LoadSimplifiedInteroperabilitySettingsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<InteroperabilitySettingEntity, InteroperabilitySettings>()
                    .ForMember(dest => dest.RegistryId, src => src.MapFrom(opt => opt.REGISTRYID))
                    .ForMember(dest => dest.RoleId, src => src.MapFrom(opt => opt.ROLEID))
                    .ForMember(dest => dest.UserId, src => src.MapFrom(opt => opt.USERID))
                    .ForMember(dest => dest.KeepPrivate, src => src.MapFrom(opt => opt.KEEPPRIVATE == 1))
                    .ForMember(dest => dest.IsEnabledInteroperability, src => src.MapFrom(opt => opt.ISENABLEDINTEROPERABILITY == 1))
                    .ForMember(dest => dest.ManagementMode, src => src.MapFrom(opt => string.IsNullOrEmpty(opt.MANAGEMENTMODE) ?
                        ManagementType.M : (ManagementType)Enum.Parse(typeof(ManagementType), opt.MANAGEMENTMODE)));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
