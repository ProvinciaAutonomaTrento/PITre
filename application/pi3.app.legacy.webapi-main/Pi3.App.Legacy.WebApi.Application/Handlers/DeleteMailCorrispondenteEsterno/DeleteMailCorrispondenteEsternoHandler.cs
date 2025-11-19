// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate.Repositories;
using Pi3.Core.AggregateModels.RuoloCorrispondenteAggregate.Repositories;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeleteMailCorrispondenteEsternoRequest = Pi3.App.Legacy.WebApi.Application.Requests.DeleteMailCorrispondenteEsterno;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DeleteMailCorrispondenteEsterno
{
    public class DeleteMailCorrispondenteEsternoHandler : IRequestHandler<DeleteMailCorrispondenteEsternoRequest, DeleteMailCorrispondenteEsternoResult>
    {
        #region Public Members

        public DeleteMailCorrispondenteEsternoHandler(ILogger<DeleteMailCorrispondenteEsternoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DeleteMailCorrispondenteEsternoResult> Handle(DeleteMailCorrispondenteEsternoRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                var idCorrispondente = request.idCorrispondente.AsLong();

                var mailCorrEsternoEntities = await this._dbContext.MailCorrEsterniEntities.AsNoTracking()
                    .Where(c => c.ID_CORR == idCorrispondente)
                    .ToListAsync();

                if (mailCorrEsternoEntities != null && mailCorrEsternoEntities.Count > 0)
                {
                    this._dbContext.MailCorrEsterniEntities.RemoveRange(mailCorrEsternoEntities);
                    await ((DbContext)_dbContext).SaveChangesAsync();
                }

                output = true;

            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                output = false;
            }

            return new DeleteMailCorrispondenteEsternoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DeleteMailCorrispondenteEsternoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
