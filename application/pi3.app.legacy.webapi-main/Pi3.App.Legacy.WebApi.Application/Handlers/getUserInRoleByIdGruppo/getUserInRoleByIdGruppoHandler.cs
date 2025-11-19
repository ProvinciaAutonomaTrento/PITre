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
using System.Text;
using System.Threading.Tasks;
using getUserInRoleByIdGruppoRequest = Pi3.App.Legacy.WebApi.Application.Requests.getUserInRoleByIdGruppo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getUserInRoleByIdGruppo
{
    public class getUserInRoleByIdGruppoHandler : IRequestHandler<getUserInRoleByIdGruppoRequest, getUserInRoleByIdGruppoResult>
    {
        #region Public Members

        public getUserInRoleByIdGruppoHandler(ILogger<getUserInRoleByIdGruppoHandler> logger,
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

        public async Task<getUserInRoleByIdGruppoResult> Handle(getUserInRoleByIdGruppoRequest request, CancellationToken cancellationToken)
        {
            Utente[] output = null;

            try
            {
                var idGruppoAsLong = request.idGruppo.AsLong();

                var peopleEntities = await this._dbContext.PeopleGroupEntities.AsNoTracking()
                    .Join(this._dbContext.PeopleEntities.AsNoTracking(), pg => pg.PEOPLE_SYSTEM_ID, people => people.SYSTEM_ID, (pg, people) => new { pg, people })
                    .Where(j => j.pg.DTA_FINE == null && j.pg.GROUPS_SYSTEM_ID == idGruppoAsLong && j.people.DISABLED == "N")
                    .OrderBy(j => j.people.FULL_NAME)
                    .Select(j => new PeopleEntity()
                    {
                        SYSTEM_ID = j.people.SYSTEM_ID,
                        FULL_NAME = j.people.FULL_NAME
                    })
                    .ToListAsync();

                output = this._mapper.Map<Utente[]>(peopleEntities);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new getUserInRoleByIdGruppoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getUserInRoleByIdGruppoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PeopleEntity, Utente>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.FULL_NAME));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
