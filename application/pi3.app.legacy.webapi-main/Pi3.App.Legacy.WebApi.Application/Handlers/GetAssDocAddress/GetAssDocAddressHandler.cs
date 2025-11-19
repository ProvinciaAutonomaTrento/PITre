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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using GetAssDocAddressRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetAssDocAddress;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetAssDocAddress
{
    public class GetAssDocAddressHandler : IRequestHandler<GetAssDocAddressRequest, GetAssDocAddressResult>
    {
        #region Public Members

        public GetAssDocAddressHandler(ILogger<GetAssDocAddressHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<GetAssDocAddressResult> Handle(GetAssDocAddressRequest request, CancellationToken cancellationToken)
        {
            DataSet output = new DataSet();
            long idProfile = Convert.ToInt64(request.docNumber);
            try
            {
                var entity = await _dbContext.AssDocMailInteropEntities.Where(a => a.ID_PROFILE == idProfile)
                    .Select(x => new { registro = x.ID_REGISTRO, mail = x.VAR_EMAIL_REGISTRO }).ToListAsync();

                output = entity.AsDataSet("ass_doc_rf", "ass_doc_rf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }
            return new GetAssDocAddressResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetAssDocAddressHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
