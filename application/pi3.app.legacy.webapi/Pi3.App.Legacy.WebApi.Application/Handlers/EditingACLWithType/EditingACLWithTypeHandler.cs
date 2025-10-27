// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EditingACLWithTypeRequest = Pi3.App.Legacy.WebApi.Application.Requests.EditingACLWithType;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.EditingACLWithType
{
    public class EditingACLWithTypeHandler : IRequestHandler<EditingACLWithTypeRequest, EditingACLWithTypeResult>
    {
        #region Public Members

        public EditingACLWithTypeHandler(ILogger<EditingACLWithTypeHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<EditingACLWithTypeResult> Handle(EditingACLWithTypeRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            try
            {
                output = (await this._mediator.Send(new Requests.EditingACL(request.docDiritto, request.personOrGroup, request.infoUtente, request.typeObject))).output;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new EditingACLWithTypeResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<EditingACLWithTypeHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }
}
