// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.amministrazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.cambiaDirittiDocumenti
{
    // Richiede libreria MediatR
    public class cambiaDirittiDocumentiHandler : IRequestHandler<Application.Requests.cambiaDirittiDocumenti, cambiaDirittiDocumentiResult>
    {
        #region Public Members

        public cambiaDirittiDocumentiHandler(ILogger<cambiaDirittiDocumentiHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IDistributedCache distributedCache)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._distributedCache = distributedCache;
        }

        public async Task<cambiaDirittiDocumentiResult> Handle(Application.Requests.cambiaDirittiDocumenti request, CancellationToken cancellationToken)
        {
            int accessRight = request.accessRight;
            var idDocumento = request.idDocumento.AsLong();
            long[] accessRights = new long[] { 0, accessRight };

            try
            {
                var securityEntities = await this._dbContext.SecurityEntities
                    .Where(s => s.THING == idDocumento)
                    .ToListAsync();

                var securityEntitiesToRemove = securityEntities.Where(s => !accessRights.Contains(s.ACCESSRIGHTS.Value)).ToList();
                
                var securityEntitiesToAdd = new List<SecurityEntity>();              
                securityEntitiesToRemove
                    .Where(s => !securityEntities.Any(x => x.PERSONORGROUP == s.PERSONORGROUP && x.ACCESSRIGHTS == accessRight))
                    .ToList()
                    .ForEach(e =>
                    {
                        if (!securityEntitiesToAdd.Any(s => s.PERSONORGROUP == e.PERSONORGROUP && s.ACCESSRIGHTS == accessRight))
                            securityEntitiesToAdd.Add(new SecurityEntity()
                            {
                                ACCESSRIGHTS = accessRight,
                                PERSONORGROUP = e.PERSONORGROUP,
                                THING = e.THING,
                                CHA_COPIA_VISIBILITA = e.CHA_COPIA_VISIBILITA,
                                CHA_TIPO_DIRITTO = e.CHA_TIPO_DIRITTO,
                                HIDE_DOC_VERSIONS = e.HIDE_DOC_VERSIONS,
                                ID_GRUPPO_TRASM = e.ID_GRUPPO_TRASM,
                                TS_INSERIMENTO = e.TS_INSERIMENTO,
                                VAR_NOTE_SEC = e.VAR_NOTE_SEC
                            });
                    });

                this._dbContext.SecurityEntities.RemoveRange(securityEntitiesToRemove);
                await this._dbContext.SecurityEntities.AddRangeAsync(securityEntitiesToAdd);
                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new cambiaDirittiDocumentiResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<cambiaDirittiDocumentiHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;

        #endregion
    }

}
