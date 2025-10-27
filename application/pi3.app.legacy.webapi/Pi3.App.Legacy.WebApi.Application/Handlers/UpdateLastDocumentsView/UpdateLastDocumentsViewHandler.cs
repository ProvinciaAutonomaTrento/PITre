// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateLastDocumentsViewRequest = Pi3.App.Legacy.WebApi.Application.Requests.UpdateLastDocumentsView;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UpdateLastDocumentsView
{
    public class UpdateLastDocumentsViewHandler : IRequestHandler<UpdateLastDocumentsViewRequest, UpdateLastDocumentsViewResult>
    {
        #region Public Members

        public UpdateLastDocumentsViewHandler(ILogger<UpdateLastDocumentsViewHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
        }

        public async Task<UpdateLastDocumentsViewResult> Handle(UpdateLastDocumentsViewRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idProfile = request.docnumber.AsLong();

                string numDocValue = (await this._configurationService.GetValue<string>(idTenant.ToString(), "BE_NUM_ULTIMI_DOC_VISUALIZZATI"));
                long maxNumUltimiDoc = !string.IsNullOrEmpty(numDocValue) ? maxNumUltimiDoc = numDocValue.AsLong() : 0;

                if (maxNumUltimiDoc != 0)
                {
                    var systemDateTime = await _dbContext.GetSystemDateTime();
                    var ultimaVis = await this._dbContext.UltimiDocVisualizzatiEntities
                        .Where(x => x.ID_PEOPLE == idUser && x.ID_GRUPPO == idGroup && x.ID_PROFILE == idProfile && x.ID_AMM == idTenant)
                        .FirstOrDefaultAsync();

                    if (ultimaVis != null)
                    {
                        ultimaVis.DTA_VISUALIZZAZIONE = systemDateTime;
                        int rowUpdated = await ((DbContext)_dbContext).SaveChangesAsync();
                    }
                    else
                    {
                        this._dbContext.UltimiDocVisualizzatiEntities.Add(new UltimiDocVisualizzatiEntity
                        {
                            ID_PEOPLE = idUser,
                            ID_GRUPPO = idGroup,
                            ID_PROFILE = idProfile,
                            DTA_VISUALIZZAZIONE = systemDateTime,
                            ID_AMM = idTenant
                        });

                        int rowIns = await ((DbContext)_dbContext).SaveChangesAsync();

                        var ultimiDoc = this._dbContext.UltimiDocVisualizzatiEntities
                            .Where(x => x.ID_PEOPLE == idUser && x.ID_GRUPPO == idGroup && x.ID_AMM == idTenant);

                        long numUltimiDoc = await ultimiDoc.CountAsync();

                        if (numUltimiDoc > maxNumUltimiDoc)
                        {
                            var minDate = await ultimiDoc.Where(x => x.ID_PEOPLE == idUser && x.ID_GRUPPO == idGroup && x.ID_AMM == idTenant).MinAsync(x => x.DTA_VISUALIZZAZIONE);
                            var entityToDelete = await ultimiDoc.Where(x => x.DTA_VISUALIZZAZIONE == minDate)
                                .FirstOrDefaultAsync();

                            if (entityToDelete != null)
                            {
                                this._dbContext.UltimiDocVisualizzatiEntities.Remove(entityToDelete);
                                int rowDeleted = await ((DbContext)_dbContext).SaveChangesAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new UpdateLastDocumentsViewResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UpdateLastDocumentsViewHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        #endregion
    }
}
