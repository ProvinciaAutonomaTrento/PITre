// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.VerificaACLHandler
{


    public class VerificaACLHandler : IRequestHandler<VerificaACL, VerificaACLResult>
    {
        #region Public Members

        public VerificaACLHandler(ILogger<VerificaACLHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<VerificaACLResult> Handle(VerificaACL request, CancellationToken cancellationToken)
        {
            var thing = request.idObj.AsLong();

            if (request.tipoObj == "D")
            {
                var profileEntity =
                    await this._dbContext.ProfileEntities
                        .AsNoTracking()
                        .Where(p => p.SYSTEM_ID == thing)
                        .Select(p =>
                        new
                        {
                            p.CHA_IN_CESTINO,
                            p.ID_DOCUMENTO_PRINCIPALE
                        })
                        .FirstAsync();

                var isInCestino = profileEntity.CHA_IN_CESTINO == "1";

                if (profileEntity.ID_DOCUMENTO_PRINCIPALE.HasValue)
                {
                    // Allegato
                    thing = profileEntity.ID_DOCUMENTO_PRINCIPALE.Value;

                    isInCestino = (this._dbContext.ProfileEntities
                            .AsNoTracking()
                            .Where(w => w.SYSTEM_ID == thing)
                            .Select(s => s.CHA_IN_CESTINO)
                            .First() == "1");
                }

                if (isInCestino)
                    return new VerificaACLResult(1, VerificaACLResources.DocInTrashCanError); //documento rimosso
            }

            var result = Convert.ToInt32(
                await _dbContext.GetSecurityRights(thing.ToString(), 
                    request.infoUtente.idPeople, 
                    request.infoUtente.idGruppo));

            switch (result)
            {
                case 0:
                    return new VerificaACLResult(0, VerificaACLResources.NoHaveRightError); //0

                case 1:
                case 2:
                case 3:
                    return new VerificaACLResult(2, string.Empty); //45,65,255

                default:
                    return new VerificaACLResult(-1, VerificaACLResources.Unrecognized_error); //eccezione del metodo
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger<VerificaACLHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
