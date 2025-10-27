// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UpdateDocArrivoFromInteropOccasionale
{
    public class UpdateDocArrivoFromInteropOccasionaleHandler : IRequestHandler<Requests.UpdateDocArrivoFromInteropOccasionale, UpdateDocArrivoFromInteropOccasionaleResult>
    {
        #region Public members

        public UpdateDocArrivoFromInteropOccasionaleHandler(ILogger<UpdateDocArrivoFromInteropOccasionaleHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<UpdateDocArrivoFromInteropOccasionaleResult> Handle(Requests.UpdateDocArrivoFromInteropOccasionale request, CancellationToken cancellationToken)
        {
            var entities = await this._dbContext.DocArrivoParEntities.Where(x => x.ID_PROFILE == request.docId.AsLong()).ToListAsync();

            entities.ForEach(x => x.ID_MITT_DEST = request.newCorrId.AsLong());

            await ((DbContext)this._dbContext).SaveChangesAsync();

            return new UpdateDocArrivoFromInteropOccasionaleResult(entities.Count);
        }

        #endregion

        #region Private members

        protected readonly ILogger<UpdateDocArrivoFromInteropOccasionaleHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
