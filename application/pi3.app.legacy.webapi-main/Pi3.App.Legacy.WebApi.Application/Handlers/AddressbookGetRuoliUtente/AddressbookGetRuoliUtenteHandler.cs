// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.rubrica;
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
using AddressbookGetRuoliUtenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddressbookGetRuoliUtente;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AddressbookGetRuoliUtente
{
    public class AddressbookGetRuoliUtenteHandler : IRequestHandler<AddressbookGetRuoliUtenteRequest, AddressbookGetRuoliUtenteResult>
    {
        #region Public Members

        public AddressbookGetRuoliUtenteHandler(ILogger<AddressbookGetRuoliUtenteHandler> logger,
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

        public async Task<AddressbookGetRuoliUtenteResult> Handle(AddressbookGetRuoliUtenteRequest request, CancellationToken cancellationToken)
        {
            ElementoRubrica[] output = null;

            try
            {
                var codRubrica = request.cod_rubrica;
                var idAmm = request.id_amm.AsLong();

                var ruoloUtenteEntities = await this._dbContext.CorrGlobaliEntities
                    .Join(this._dbContext.PeopleEntities, u => u.ID_PEOPLE, p => p.SYSTEM_ID, (u, p) => new { u, p })
                    .Join(this._dbContext.PeopleGroupEntities, j => j.p.SYSTEM_ID, pg => pg.PEOPLE_SYSTEM_ID, (j, pg) => new { j.u, j.p, pg })
                    .Join(this._dbContext.CorrGlobaliEntities, j => j.pg.GROUPS_SYSTEM_ID, r => r.ID_GRUPPO, (j, r) => new { j.u, j.p, j.pg, r })
                    .Where(j => j.u.CHA_TIPO_URP == "P" && j.u.DTA_FINE == null && j.r.DTA_FINE == null && j.pg.DTA_FINE == null && j.u.VAR_COD_RUBRICA == codRubrica && j.u.ID_AMM == idAmm)
                    .OrderBy(j => j.u.VAR_COD_RUBRICA)
                    .Select(j => new RuoliUtenteEntity()
                    {
                        COD_UTENTE = j.u.VAR_COD_RUBRICA,
                        COD_RUOLO = j.r.VAR_COD_RUBRICA,
                        DESC_RUOLO = j.r.VAR_DESC_CORR,
                        INTERNO = "1",
                        TIPO = "R",
                        HAS_CHILDREN = "1"
                    })
                    .AsNoTracking()
                    .ToListAsync();

                output = this._mapper.Map<ElementoRubrica[]>(ruoloUtenteEntities);
            }
            catch (Exception ex) 
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new AddressbookGetRuoliUtenteResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AddressbookGetRuoliUtenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RuoliUtenteEntity, ElementoRubrica>()
                    .ForMember(dest => dest.codice, opt => opt.MapFrom(src => src.COD_RUOLO))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.DESC_RUOLO))
                    .ForMember(dest => dest.interno, opt => opt.MapFrom(src => src.INTERNO == "1"))
                    .ForMember(dest => dest.tipo, opt => opt.MapFrom(src => src.TIPO))
                    .ForMember(dest => dest.has_children, opt => opt.MapFrom(src => src.HAS_CHILDREN == "1"));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class RuoliUtenteEntity
        { 
            public string? COD_UTENTE { get; set; }
            public string? COD_RUOLO { get; set; }
            public string? DESC_RUOLO { get; set; }
            public string? INTERNO { get; set; }
            public string? TIPO { get; set; }
            public string? HAS_CHILDREN { get; set; }

        }

        #endregion
    }
}
