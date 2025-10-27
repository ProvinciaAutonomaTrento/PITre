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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UpdateDocArrivoFromInterop
{
    public class UpdateDocArrivoFromInteropHandler : IRequestHandler<Requests.UpdateDocArrivoFromInterop, UpdateDocArrivoFromInteropResult>
    {
        #region Public members

        public UpdateDocArrivoFromInteropHandler(ILogger<UpdateDocArrivoFromInteropHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<UpdateDocArrivoFromInteropResult> Handle(Requests.UpdateDocArrivoFromInterop request, CancellationToken cancellationToken)
        {
            var entities = await this._dbContext.DocArrivoParEntities
                .Join(this._dbContext.ProfileEntities, d => d.ID_PROFILE, p => p.SYSTEM_ID, (d, p) => new { d, p })
                .Where(x => !x.p.NUM_PROTO.HasValue
                    && x.p.CHA_TIPO_PROTO == "A"
                    && x.p.CHA_DA_PROTO == "1"
                    && x.d.ID_MITT_DEST == request.oldCorrId.AsLong()
                    )
                .Select(x => x.d)
                .ToListAsync();

            entities.ForEach(x=> x.ID_MITT_DEST = request.newCorrId.AsLong());

            await ((DbContext)this._dbContext).SaveChangesAsync();

            return new UpdateDocArrivoFromInteropResult(entities.Count);
        }

        #endregion

        #region Private members

        protected readonly ILogger<UpdateDocArrivoFromInteropHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
