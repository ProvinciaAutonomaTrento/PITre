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
using RemoveReportMailboxRequest = Pi3.App.Legacy.WebApi.Application.Requests.RemoveReportMailbox;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RemoveReportMailbox
{

    public class RemoveReportMailboxHandler : IRequestHandler<RemoveReportMailboxRequest, RemoveReportMailboxResult>
    {
        #region Public Members

        public RemoveReportMailboxHandler(ILogger<RemoveReportMailboxHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<RemoveReportMailboxResult> Handle(RemoveReportMailboxRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            var idCheckMailbox = request.idCheckMailbox.AsLong();

            try
            {
                var checkMailboxEntity = await this._dbContext.CheckMailboxEntities.AsNoTracking().FirstOrDefaultAsync(c => c.ID == idCheckMailbox);
                if (checkMailboxEntity == null)
                    throw new ReportMailboxNotFoundPi3Exception(request.idCheckMailbox);

                this._dbContext.CheckMailboxEntities.Remove(checkMailboxEntity);
                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            catch(Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new RemoveReportMailboxResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RemoveReportMailboxHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
