// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
using System.Text;
using System.Threading.Tasks;
using CheckExistsRoleSupByTypeRolesRequest = Pi3.App.Legacy.WebApi.Application.Requests.CheckExistsRoleSupByTypeRoles;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckExistsRoleSupByTypeRoles
{
    public class CheckExistsRoleSupByTypeRolesHandler : IRequestHandler<CheckExistsRoleSupByTypeRolesRequest, CheckExistsRoleSupByTypeRolesResult>
    {
        #region Public Members

        public CheckExistsRoleSupByTypeRolesHandler(ILogger<CheckExistsRoleSupByTypeRolesHandler> logger,
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

        public async Task<CheckExistsRoleSupByTypeRolesResult> Handle(CheckExistsRoleSupByTypeRolesRequest request, CancellationToken cancellationToken)
        {
            var idGruppoAsLong = request.infoUtente.idGruppo.AsLong();
            var idTipiRuoloAsLong = request.typeRole.Select(t => t.systemId.AsLong()).ToList();

            var idUO = await _dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppoAsLong).Select(c => c.ID_PARENT).FirstAsync();

            List<long> idsUO = new List<long>();
            var idUOParent = idUO;
            while (idUOParent != null)
            {
                idsUO.Add(idUOParent.Value);
                idUOParent = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.ID_PARENT == idUOParent && c.DTA_FINE == null)
                    .Select(c => c.SYSTEM_ID)
                    .FirstOrDefaultAsync();
            }

            var tipoRuoloEntity = await _dbContext.TipoRuoloEntities.AsNoTracking()
                .Where(t => idTipiRuoloAsLong.Contains(t.SYSTEM_ID)
                    && !_dbContext.CorrGlobaliEntities.Any(c => c.DTA_FINE == null
                        && c.ID_TIPO_RUOLO == t.SYSTEM_ID
                        && idsUO.Contains(c.ID_UO.Value)))
                .Select(t => t)
                .ToListAsync();

            var output = _mapper.Map<TipoRuolo[]>(tipoRuoloEntity);

            return new CheckExistsRoleSupByTypeRolesResult(output);

        }

        #endregion

        #region Private Members

        protected readonly ILogger<CheckExistsRoleSupByTypeRolesHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TipoRuoloEntity, TipoRuolo>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_RUOLO))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.VAR_CODICE));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion

    }
}