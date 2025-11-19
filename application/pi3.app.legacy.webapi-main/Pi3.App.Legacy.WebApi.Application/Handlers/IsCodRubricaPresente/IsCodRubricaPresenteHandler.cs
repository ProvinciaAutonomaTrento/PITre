// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IsCodRubricaPresenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsCodRubricaPresente;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsCodRubricaPresente
{
    public class IsCodRubricaPresenteHandler : IRequestHandler<IsCodRubricaPresenteRequest, IsCodRubricaPresenteResult>
    {

        protected readonly ILogger<IsCodRubricaPresenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        public IsCodRubricaPresenteHandler(
            ILogger<IsCodRubricaPresenteHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._logger = logger;
        }


        public async Task<IsCodRubricaPresenteResult> Handle(IsCodRubricaPresenteRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                output = (await _mediator.Send(new Requests.CheckCodRubricaPresente(request.codRubrica, request.tipoCorr, request.idAmm, request.idReg, request.inRubricaComune))).output;
            }
            catch(Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                output = true;
            }

            return new IsCodRubricaPresenteResult(output);
        }

    }
}
