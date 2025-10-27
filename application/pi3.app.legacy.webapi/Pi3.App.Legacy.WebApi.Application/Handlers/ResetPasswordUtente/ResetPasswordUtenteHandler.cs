// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Validations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResetPasswordUtenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.ResetPasswordUtente;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ResetPasswordUtente
{
    public class ResetPasswordUtenteHandler : IRequestHandler<ResetPasswordUtenteRequest, ResetPasswordUtenteResult>
    {
        #region Public Members

        public ResetPasswordUtenteHandler(ILogger<ResetPasswordUtenteHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<ResetPasswordUtenteResult> Handle(ResetPasswordUtenteRequest request, CancellationToken cancellationToken)
        {
            List<BrokenRule> brokenRules = new List<BrokenRule>();
            var userId = request.userLogin.UserName.ToUpper();

            try
            {
                if(!await _dbContext.PeopleOtpResetPasswordEntities.AsNoTracking()
                    .AnyAsync(p => p.USER_ID_PEOPLE.ToUpper().Equals(userId) && p.VAR_OTP.Equals(request.otp)))
                {
                    brokenRules.Add(new BrokenRule("INVALID_OTP", Resources.OtpErrato, BrokenRule.BrokenRuleLevelEnum.Error));
                    throw new InvalidOTPPi3Exception();
                }

                brokenRules = (await _mediator.Send(new Requests.UserChangePassword(request.userLogin, string.Empty))).output.BrokenRules.ToList();

            }
            catch(Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                brokenRules.Add(new BrokenRule("ChangePassword_ERROR", Resources.ErroreModificaPassword, BrokenRule.BrokenRuleLevelEnum.Error));
            }

            return new ResetPasswordUtenteResult(
               new ValidationResultInfo()
               {
                   Value = !brokenRules.Any(),
                   BrokenRules = brokenRules.ToArray()
               });
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ResetPasswordUtenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}