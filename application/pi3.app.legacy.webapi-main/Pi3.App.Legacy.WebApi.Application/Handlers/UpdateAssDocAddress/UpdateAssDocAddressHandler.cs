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

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.UpdateAssDocAddress
{

    public class UpdateAssDocAddressHandler : IRequestHandler<Application.Requests.UpdateAssDocAddress, UpdateAssDocAddressResult>
    {
        #region Public Members

        public UpdateAssDocAddressHandler(ILogger<UpdateAssDocAddressHandler> logger, IMediator mediator, IClaimsPrincipalService claimsPrincipalService, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<UpdateAssDocAddressResult> Handle(Application.Requests.UpdateAssDocAddress request, CancellationToken cancellationToken)
        {
            long docNumber = long.Parse(request.docNumber);
            long idRegistro = long.Parse(request.idRegistro);
            bool result = false;

            var toUpdate = _dbContext.AssDocMailInteropEntities.Where(m => m.ID_PROFILE == docNumber).ToList();

            if((toUpdate == null))
            {
                return new UpdateAssDocAddressResult(false);
            }

            foreach(var update in toUpdate)
            {
                update.ID_REGISTRO = idRegistro;
                update.VAR_EMAIL_REGISTRO = request.mailAddress;
            }

            if(await((DbContext)this._dbContext).SaveChangesAsync(cancellationToken)>0)
            {
                result = true;
            }

            return new UpdateAssDocAddressResult(result);
        }

        #endregion

        #region Private Members
        protected readonly ILogger<UpdateAssDocAddressHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IPi3DbContext _dbContext;
        #endregion
    }

}
