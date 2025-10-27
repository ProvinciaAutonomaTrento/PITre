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
using DocumentoCancellaAreaLavoroRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoCancellaAreaLavoro;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoCancellaAreaLavoro
{

    public class DocumentoCancellaAreaLavoroHandler : IRequestHandler<DocumentoCancellaAreaLavoroRequest, DocumentoCancellaAreaLavoroResult>
    {
        #region Public Members

        public DocumentoCancellaAreaLavoroHandler(ILogger<DocumentoCancellaAreaLavoroHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DocumentoCancellaAreaLavoroResult> Handle(DocumentoCancellaAreaLavoroRequest request, CancellationToken cancellationToken)
        {
            bool output = true;
            var idPeopleAsLong = request.idPeople.AsLong();
            var idRuoloInUOAsLong = request.idRuoloInUO.AsLong();

            try
            {
                var query = this._dbContext.AreaLavoroEntities.Where(a => a.ID_PEOPLE == idPeopleAsLong && a.ID_RUOLO_IN_UO == idRuoloInUOAsLong);
                if(!string.IsNullOrEmpty(request.idProfile))
                {
                    var idProfileAsLong = request.idProfile.AsLong();
                    query = query.Where(a => a.ID_PROFILE == idProfileAsLong);
                }
                else
                {
                    var idProjectAsLong = request.fasc.systemID.AsLong();
                    query = query.Where(a => a.ID_PROJECT == idProjectAsLong);
                }

                var areaLavoroEntity = await query.FirstOrDefaultAsync();
                if (areaLavoroEntity != null)
                {
                    this._dbContext.AreaLavoroEntities.Remove(areaLavoroEntity);
                    await ((DbContext)_dbContext).SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = false;
            }

            return new DocumentoCancellaAreaLavoroResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoCancellaAreaLavoroHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
