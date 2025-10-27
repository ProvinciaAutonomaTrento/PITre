// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ProfilazioneDinamicaLite;
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
using ValidateLoginRequest = Pi3.App.Legacy.WebApi.Application.Requests.ValidateLogin;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ValidateLogin
{
    public class ValidateLoginHandler : IRequestHandler<ValidateLoginRequest, ValidateLoginResult>
    {
        #region Public Members

        public ValidateLoginHandler(
            ILogger<ValidateLoginHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<ValidateLoginResult> Handle(ValidateLoginRequest request, CancellationToken cancellationToken)
        {
            var output = DocsPaVO.utente.UserLogin.ValidationResult.OK;

            try
            {
                var sessionIdAlreadyExists = await this._pi3DbContext
                        .LoginEntities
                        .AsNoTracking()
                     .Where(l => l.SESSION_ID.ToUpper() == request.webSessionId.ToUpper()
                             && l.USER_ID.ToUpper() == request.userID.ToUpper()
                             && l.ID_AMM == request.idAmm.AsLong())
                        .AnyAsync();
                
                if (!sessionIdAlreadyExists)
                {
                    var userIdExists = await this._pi3DbContext
                        .LoginEntities
                        .AsNoTracking()
                        .Where(l => l.USER_ID.ToUpper() == request.userID.ToUpper()
                            && l.ID_AMM == request.idAmm.AsLong())
                        .AnyAsync();

                    if (userIdExists)
                        output = DocsPaVO.utente.UserLogin.ValidationResult.SESSION_DROPPED;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogCritical("Errore durante la validazione della sessione web.", ex);

                output = DocsPaVO.utente.UserLogin.ValidationResult.APPLICATION_ERROR;
            }

            return new  ValidateLoginResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ValidateLoginHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }
}