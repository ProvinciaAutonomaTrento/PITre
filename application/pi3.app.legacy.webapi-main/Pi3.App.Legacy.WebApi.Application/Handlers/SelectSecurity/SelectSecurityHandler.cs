// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
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
using SelectSecurityRequest = Pi3.App.Legacy.WebApi.Application.Requests.SelectSecurity;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SelectSecurity
{
    public class SelectSecurityHandler : IRequestHandler<SelectSecurityRequest, SelectSecurityResult>
    {
        #region Public Members

        public SelectSecurityHandler(ILogger<SelectSecurityHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<SelectSecurityResult> Handle(SelectSecurityRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            string accessRight = string.Empty;
            string idGruppoTrasm = string.Empty;
            string tipoDiritto = string.Empty;

            try
            {
                var thing = request.thing.AsLong();
                var person_or_groups = request.personOrgroup.AsLong();
                var entities = _dbContext.SecurityEntities.Where(s => s.THING == thing && s.PERSONORGROUP == person_or_groups);
                if (!string.IsNullOrEmpty(request.accessRightsToTest))
                {
                    string[] split = request.accessRightsToTest.Split("=");
                    if(split.Length > 1)
                    {
                        var accessrights = split[1].Trim().AsLong();
                        entities = entities.Where(s => s.ACCESSRIGHTS == accessrights);
                    }
                }

                entities.ToList().ForEach(x =>
                    {
                        accessRight = x.ACCESSRIGHTS.ToString();
                        idGruppoTrasm = x.ID_GRUPPO_TRASM.ToString();
                        tipoDiritto = x.CHA_TIPO_DIRITTO;
                        output = true;
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new SelectSecurityResult(output, accessRight, idGruppoTrasm, tipoDiritto);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SelectSecurityHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
