// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.utente;
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GetRoleHistoryRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetRoleHistory;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetRoleHistory
{
    public class GetRoleHistoryHandler : IRequestHandler<GetRoleHistoryRequest, GetRoleHistoryResult>
    {
        #region Public Members

        public GetRoleHistoryHandler(ILogger<GetRoleHistoryHandler> logger,
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

        public async Task<GetRoleHistoryResult> Handle(GetRoleHistoryRequest request, CancellationToken cancellationToken)
        {
            RoleHistoryResponse output = new RoleHistoryResponse();

            try
            {
                var idCorrGlobali = request.request.IdCorrGlobRole.AsLong();

                var roleHistoryEntities = await this._dbContext.RoleHistoryEntities
                    .Join(this._dbContext.CorrGlobaliEntities, history => history.UO_ID, uo => uo.SYSTEM_ID, (history, uo) => new { history, uo })
                    .Join(this._dbContext.TipoRuoloEntities, j => j.history.ROLE_TYPE_ID, tipoRuolo => tipoRuolo.SYSTEM_ID, (j, tipoRuolo) => new { j.history, j.uo, tipoRuolo })
                    .Where(j => (this._dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == idCorrGlobali).Select(c => c.ORIGINAL_ID).ToList()).Contains(j.history.ORIGINAL_CORR_ID))
                    .OrderByDescending(j => j.history.ACTION_DATE)
                    .Select(j => new RoleHistoryEntity()
                    {
                        ORIGINAL_CORR_ID = j.history.ORIGINAL_CORR_ID,
                        ACTION = j.history.ACTION,
                        ROLE_DESCRIPTION = j.history.ROLE_DESCRIPTION,
                        UO_DESCRIPTION = string.Format("{0} ({1})", j.uo.VAR_DESC_CORR, j.uo.VAR_CODICE),
                        ROLE_TYPE_DESCRIPTION = string.Format("{0} ({1})", j.tipoRuolo.VAR_DESC_RUOLO, j.tipoRuolo.VAR_CODICE),
                        ROLE_ID = j.history.ROLE_ID,
                        ACTION_DATE = j.history.ACTION_DATE
                    })
                    .AsNoTracking()
                    .ToListAsync();

                output.RoleHistoryItems = this._mapper.Map<RoleHistoryItem[]>(roleHistoryEntities).ToList();
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new GetRoleHistoryResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetRoleHistoryHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RoleHistoryEntity, RoleHistoryItem>()
                    .ForMember(dest => dest.HistoryAction, opt => opt.MapFrom(src => (RoleHistoryItem.AdmittedHistoryAction)Enum.Parse(typeof(RoleHistoryItem.AdmittedHistoryAction), src.ACTION)))
                    .ForMember(dest => dest.ActionDate, opt => opt.MapFrom(src => src.ACTION_DATE))
                    .ForMember(dest => dest.OriginalCorrId, opt => opt.MapFrom(src => src.ORIGINAL_CORR_ID))
                    .ForMember(dest => dest.RoleDescription, opt => opt.MapFrom(src => src.ROLE_DESCRIPTION))
                    .ForMember(dest => dest.RoleTypeDescription, opt => opt.MapFrom(src => src.ROLE_TYPE_DESCRIPTION))
                    .ForMember(dest => dest.UoDescription, opt => opt.MapFrom(src => src.UO_DESCRIPTION))
                    .ForMember(dest => dest.RoleCorrGlobId, opt => opt.MapFrom(src => src.ROLE_ID));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class RoleHistoryEntity
        {
            public long? ORIGINAL_CORR_ID { get; set; }
            public long? ROLE_ID { get; set; }
            public string? ACTION { get; set; }
            public string? ROLE_DESCRIPTION { get; set; }
            public DateTime? ACTION_DATE { get; set; }
            public string? UO_DESCRIPTION { get; set; }
            public string? ROLE_TYPE_DESCRIPTION { get; set; }
        }
        #endregion
    }
}
