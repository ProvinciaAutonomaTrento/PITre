// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetFascicolazionePrimaria
{

    // Richiede libreria MediatR
    public class GetFascicolazionePrimariaHandler : IRequestHandler<Application.Requests.GetFascicolazionePrimaria, GetFascicolazionePrimariaResult>
    {
        #region Public Members

        public GetFascicolazionePrimariaHandler(ILogger<GetFascicolazionePrimariaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;

        }

        public async Task<GetFascicolazionePrimariaResult> Handle(Application.Requests.GetFascicolazionePrimaria request, CancellationToken cancellationToken)
        {
            string fascPrimaria = string.Empty;

            try
            {

                Microsoft.EntityFrameworkCore.DbSet<ProjectEntity> projectEntities = _dbContext.ProjectEntities;
                Microsoft.EntityFrameworkCore.DbSet<ProjectComponentEntity> projectComponentEntities = _dbContext.ProjectComponentEntities;
                long idProfile = Convert.ToInt64(request.idProfile);

                //var rights = Convert.ToInt32(await _dbContext.GetSecurityRights(request.idProfile, request.infoUtente.idPeople, request.infoUtente.idGruppo));
                var idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser).ToString();
                var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup).ToString();
                var rights = Convert.ToInt32(await _dbContext.GetSecurityRights(request.idProfile, idUser, idGroup));

                var idFascicolo = projectComponentEntities
                    .Where(x => x.LINK == idProfile && x.CHA_FASC_PRIMARIA.Equals("1"))
                    .Join(
                    projectEntities,
                    projectComponentEntities => projectComponentEntities.PROJECT_ID,
                    projectEntities => projectEntities.SYSTEM_ID,
                    (projectComponentEntities, projectEntities) => new
                    {
                        IdFascicolo = projectEntities.ID_FASCICOLO
                    })
                    .Select(x => x.IdFascicolo)
                    .FirstOrDefault();

                if (idFascicolo != null)
                {
                    string code = string.Empty;
                    string description = string.Empty;

                    code = projectEntities.Where(x => x.SYSTEM_ID == idFascicolo).Select(x => x.VAR_CODICE).FirstOrDefault();

                    switch (rights)
                    {
                        case 0:
                            description = "Descrizione non visualizzabile";
                            break;
                        default:
                            description = projectEntities.Where(x => x.SYSTEM_ID == idFascicolo).Select(x => x.DESCRIPTION).FirstOrDefault();
                            break;
                    }
                    fascPrimaria = string.Format("{0} - {1}", code, description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new GetFascicolazionePrimariaResult(fascPrimaria);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetFascicolazionePrimariaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
