// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.GetEventNotification;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetMailPrincipaleRegistroRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetMailPrincipaleRegistro;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetMailPrincipaleRegistro
{
    public class GetMailPrincipaleRegistroHandler : IRequestHandler<GetMailPrincipaleRegistroRequest, GetMailPrincipaleRegistroResult>
    {
        #region Public Members

        public GetMailPrincipaleRegistroHandler(ILogger<GetMailPrincipaleRegistroHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetMailPrincipaleRegistroResult> Handle(GetMailPrincipaleRegistroRequest request, CancellationToken cancellationToken)
        {
            var output = string.Empty;

            try
            {
                var idRegistroAsLong = request.idRegistro.AsLong();

                output = await this._dbContext.MailRegistriEntities.AsNoTracking()
                    .Where(m => m.ID_REGISTRO == idRegistroAsLong && m.VAR_PRINCIPALE == "1")
                    .Select(m => m.VAR_EMAIL_REGISTRO)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new GetMailPrincipaleRegistroResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetMailPrincipaleRegistroHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
