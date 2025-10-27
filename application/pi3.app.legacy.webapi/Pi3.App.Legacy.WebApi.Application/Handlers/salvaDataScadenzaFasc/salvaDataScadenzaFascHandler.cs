// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using salvaDataScadenzaFascRequest = Pi3.App.Legacy.WebApi.Application.Requests.salvaDataScadenzaFasc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.salvaDataScadenzaFasc
{
    public class salvaDataScadenzaFascHandler : IRequestHandler<salvaDataScadenzaFascRequest, salvaDataScadenzaFascResult>
    {
        #region Public Members

        public salvaDataScadenzaFascHandler(ILogger<salvaDataScadenzaFascHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IWebMethodLoggerService webMethodLoggerService,
            IPi3DbContext dbContext,
            IAggregazioneDocumentaleRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._webMethodLoggerService = webMethodLoggerService;
            this._dbContext = dbContext;
            this._repository = repository;
        }

        public async Task<salvaDataScadenzaFascResult> Handle(salvaDataScadenzaFascRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var systemIdFascicoloAsLong = request.idProject.AsLong();
                var idFolderFascicolo = await this._dbContext.ProjectEntities.AsNoTracking()
                    .Where(p => p.ID_PARENT == systemIdFascicoloAsLong)
                    .Select(p => p.SYSTEM_ID)
                    .FirstOrDefaultAsync();

                var aggregate = await this._repository.Get(idTenant, idFolderFascicolo.ToString(), new ILoadBehavior[1]
                {
                    new GetAggregatoDocumentaleLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadClassifications = true,
                        LoadFolderHierarchy = true,
                        LoadDocuments = false,
                        LoadNote = false,
                        LoadPermissions = false,
                        LoadProfilesMetadata = false,
                    }
                });

                DateTime? dtaScadenza = null;
                if (string.IsNullOrEmpty(request.dataScadenza))
                {
                    var idTipoFascAsLong = request.idTipoFasc.AsLong();
                    var scadenza = await this._dbContext.TipoFascEntities.AsNoTracking().Where(t => t.SYSTEM_ID == idTipoFascAsLong).Select(t => t.GG_SCADENZA).FirstOrDefaultAsync();
                    if (scadenza != null && scadenza != 0)
                        dtaScadenza = (await this._dbContext.GetSystemDateTime()).AddDays((double)scadenza).Date;
                }

                if (!string.IsNullOrEmpty(request.dataScadenza))
                {
                    dtaScadenza = request.dataScadenza.AsDateTime().Date;
                }

                aggregate.AssignDataScadenza(dtaScadenza);

                await this._repository.Update(aggregate);

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new salvaDataScadenzaFascResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<salvaDataScadenzaFascHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IAggregazioneDocumentaleRepository _repository;

        #endregion
    }
}
