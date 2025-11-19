// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getIdTemplateRequest = Pi3.App.Legacy.WebApi.Application.Requests.getIdTemplate;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getIdTemplate
{

    public class getIdTemplateHandler : IRequestHandler<getIdTemplateRequest, getIdTemplateResult>
    {
        #region Public Members

        public getIdTemplateHandler(ILogger<getIdTemplateHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<getIdTemplateResult> Handle(getIdTemplateRequest request, CancellationToken cancellationToken)
        {
            string output = string.Empty;

            try
            {
                long docnumber = Convert.ToInt64(request.docNumber);
                var idTipoAtto = await _dbContext.ProfileEntities.Where(p => p.DOCNUMBER == docnumber).Select(p => p.ID_TIPO_ATTO).FirstAsync();

                if (idTipoAtto != null)
                    output = idTipoAtto.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new getIdTemplateResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getIdTemplateHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
