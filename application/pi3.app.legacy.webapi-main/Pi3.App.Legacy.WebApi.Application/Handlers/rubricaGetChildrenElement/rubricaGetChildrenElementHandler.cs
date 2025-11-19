// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.rubrica;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rubricaGetChildrenElementRequest = Pi3.App.Legacy.WebApi.Application.Requests.rubricaGetChildrenElement;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.rubricaGetChildrenElement
{
    public class rubricaGetChildrenElementHandler : IRequestHandler<rubricaGetChildrenElementRequest, rubricaGetChildrenElementResult>
    {
        #region Public Members

        public rubricaGetChildrenElementHandler(ILogger<rubricaGetChildrenElementHandler> logger,
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

        public async Task<rubricaGetChildrenElementResult> Handle(rubricaGetChildrenElementRequest request, CancellationToken cancellationToken)
        {
            ElementoRubrica[] output = null;
            List<CorrGlobaliEntity> corrGlobaliEntities = null;
            try
            {
                var idCorrGlobali = request.elementID.AsLong();
                if(request.childrensType.ToUpper() == "R")
                {
                    corrGlobaliEntities = await this._dbContext.PeopleGroupEntities.AsNoTracking()
                        .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(), pg => pg.GROUPS_SYSTEM_ID, r => r.ID_GRUPPO, (pg, r) => new { pg, r })
                        .Join(this._dbContext.PeopleEntities.AsNoTracking(), j => j.pg.PEOPLE_SYSTEM_ID, p => p.SYSTEM_ID, (j, p) => new { j.pg, j.r, p } )
                        .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(), j => j.p.SYSTEM_ID, u => u.ID_PEOPLE, (j, u) => new { j.pg, j.r, u })
                        .Where(j => j.pg.DTA_FINE == null && j.r.SYSTEM_ID == idCorrGlobali)
                        .Select(j => new CorrGlobaliEntity()
                        {
                            VAR_COD_RUBRICA = j.u.VAR_COD_RUBRICA,
                            VAR_DESC_CORR = j.u.VAR_DESC_CORR,
                            CHA_TIPO_IE = j.u.CHA_TIPO_IE,
                            CHA_TIPO_URP = j.u.CHA_TIPO_URP,
                            SYSTEM_ID = j.u.SYSTEM_ID
                        })
                        .ToListAsync();
                }
                else
                {
                    corrGlobaliEntities = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.CHA_TIPO_URP == "R" && c.DTA_FINE == null && c.ID_UO == idCorrGlobali)
                        .Select(c => new CorrGlobaliEntity()
                        {
                            VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                            VAR_DESC_CORR = c.VAR_DESC_CORR,
                            CHA_TIPO_IE = c.CHA_TIPO_IE,
                            CHA_TIPO_URP = c.CHA_TIPO_URP,
                            SYSTEM_ID = c.SYSTEM_ID
                        })
                        .ToListAsync();
                }

                if(corrGlobaliEntities != null)
                    output = _mapper.Map<ElementoRubrica[]>(corrGlobaliEntities);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new rubricaGetChildrenElementResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<rubricaGetChildrenElementHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;
        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CorrGlobaliEntity, ElementoRubrica>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_CORR))
                    .ForMember(dest => dest.interno, src => src.MapFrom(opt => opt.CHA_TIPO_IE == "I"))
                    .ForMember(dest => dest.tipo, src => src.MapFrom(opt => opt.CHA_TIPO_URP))
                    .ForMember(dest => dest.has_children, src => src.MapFrom(opt => false));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
