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
using Albo_InsFileDaPubblicareRequest = Pi3.App.Legacy.WebApi.Application.Requests.Albo_InsFileDaPubblicare;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.Albo_InsFileDaPubblicare
{
    public class Albo_InsFileDaPubblicareHandler : IRequestHandler<Albo_InsFileDaPubblicareRequest, Albo_InsFileDaPubblicareResult>
    {
        #region Public Members

        public Albo_InsFileDaPubblicareHandler(ILogger<Albo_InsFileDaPubblicareHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<Albo_InsFileDaPubblicareResult> Handle(Albo_InsFileDaPubblicareRequest request, CancellationToken cancellationToken)
        {
            bool output = true;

            try
            {
                var docnumberAsLong = request.docNumber.AsLong();
                var idDocumentoPrincipaleAsLong = request.idDocPrincipale.AsLong();

                var alboDocPubbEntity = await this._dbContext.AlboDocPubbEntities.FirstOrDefaultAsync(a => a.DOCNUMBER == docnumberAsLong);
                if(alboDocPubbEntity != null)
                {
                    alboDocPubbEntity.DA_PUBB = request.daPubb;
                }
                else
                {
                    alboDocPubbEntity = new AlboDocPubbEntity()
                    {
                        DOCNUMBER = docnumberAsLong,
                        ID_DOC_PRINCIPALE = idDocumentoPrincipaleAsLong,
                        DA_PUBB = request.daPubb
                    };

                    await this._dbContext.AlboDocPubbEntities.AddAsync(alboDocPubbEntity);
                }

                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new Albo_InsFileDaPubblicareResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<Albo_InsFileDaPubblicareHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
