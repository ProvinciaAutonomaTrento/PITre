// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using UpdateSizeItemConsRequest = Pi3.App.Legacy.WebApi.Application.Requests.UpdateSizeItemCons;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UpdateSizeItemCons
{
    public class UpdateSizeItemConsHandler : IRequestHandler<UpdateSizeItemConsRequest, UpdateSizeItemConsResult>
    {
        #region Public Members

        public UpdateSizeItemConsHandler(ILogger<UpdateSizeItemConsHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<UpdateSizeItemConsResult> Handle(UpdateSizeItemConsRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var systemIdAsLong = request.sysId.AsLong();
                var itemsConservazioneEntity = await this._dbContext.ItemConservazioneEntities.FirstAsync(i => i.SYSTEM_ID == systemIdAsLong);

                if (itemsConservazioneEntity != null)
                {
                    itemsConservazioneEntity.SIZE_ITEM = request.size;
                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = false;
            }

            return new UpdateSizeItemConsResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UpdateSizeItemConsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
