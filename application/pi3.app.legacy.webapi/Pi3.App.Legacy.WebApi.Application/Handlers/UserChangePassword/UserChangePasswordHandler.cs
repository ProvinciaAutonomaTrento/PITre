// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.Validations;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserChangePasswordRequest = Pi3.App.Legacy.WebApi.Application.Requests.UserChangePassword;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UserChangePassword
{
    public class UserChangePasswordHandler : IRequestHandler<UserChangePasswordRequest, UserChangePasswordResult>
    {
        #region Public Members

        public UserChangePasswordHandler(
            ILogger<UserChangePasswordHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<UserChangePasswordResult> Handle(UserChangePasswordRequest request, CancellationToken cancellationToken)
        {
            var brokenRules = new List<BrokenRule>();
            var wasErrors = false;

            try
            {
                var peopleEntities = await this._pi3DbContext.PeopleEntities
                    .Where(p => p.USER_ID.ToUpper() == request.user.UserName.ToUpper())
                    .ToListAsync();

                var isNotMultiAmm = peopleEntities != null && peopleEntities.Count == 1;

                if (peopleEntities == null || peopleEntities.Count < 1)
                {
                    brokenRules.Add(new BrokenRule("USER_NOT_FOUND",
                        ErrorDescriptions.UserNotFound, BrokenRule.BrokenRuleLevelEnum.Error));
                    throw new UserNotFoundPi3Exception();
                }
                else
                {
                    var newEncryptedPassword = this.EncryptPassword(request.user.Password);

                    foreach (var peopleEntity in peopleEntities)
                    {
                        if (string.Compare(peopleEntity.ENCRYPTED_PASSWORD, newEncryptedPassword, false) == 0)
                        {
                            brokenRules.Add(new BrokenRule("PASSWORD_EQUALITY",
                                ErrorDescriptions.PasswordEquality, BrokenRule.BrokenRuleLevelEnum.Error));
                            throw new PasswordEqualityPi3Exception();
                        }

                        if (isNotMultiAmm)
                            await this.CheckPasswordRules(
                                brokenRules,
                                request.user.Password,
                                Convert.ToInt32(peopleEntity.ID_AMM));

                        if (brokenRules.Count == 0)
                        {
                            peopleEntity.USER_PASSWORD = null;
                            peopleEntity.ENCRYPTED_PASSWORD = newEncryptedPassword;
                            peopleEntity.PASSWORD_CREATION_DATE = DateTime.Now;
                        }
                    }
                    await ((DbContext)this._pi3DbContext).SaveChangesAsync();
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                wasErrors = true;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                wasErrors = true;
                brokenRules.Add(new BrokenRule("ChangePassword_ERROR",
                        ErrorDescriptions.ChangePasswordError, DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error));
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new UserChangePasswordResult(
                new ValidationResultInfo()
                {
                    Value = !brokenRules.Any(),
                    BrokenRules = brokenRules.ToArray()
                });
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UserChangePasswordHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        private async Task CheckPasswordRules(List<BrokenRule> brokenRules, string clearPassword, int idAmministrazione)
        {
            var passwordConfigurations = await this._pi3DbContext.AmministraEntities
                .AsNoTracking()
                .Where(a => a.SYSTEM_ID == idAmministrazione)
                .Select(a => new DocsPaVO.amministrazione.PasswordConfigurations()
                {
                    IdAmministrazione = idAmministrazione,
                    ExpirationEnabled = a.ENABLE_PASSWORD_EXPIRATION == "1",
                    ValidityDays = !string.IsNullOrWhiteSpace(a.PASSWORD_EXPIRATION_DAYS) ? Convert.ToInt32(a.PASSWORD_EXPIRATION_DAYS) : 0,
                    MinLength = !string.IsNullOrWhiteSpace(a.PASSWORD_MIN_LENGTH) ? Convert.ToInt32(a.PASSWORD_MIN_LENGTH) : 0,
                    SpecialCharacters = !string.IsNullOrWhiteSpace(a.PASSWORD_SPECIAL_CHAR_LIST) ? a.PASSWORD_SPECIAL_CHAR_LIST.ToCharArray() : new char[0]
                })
                .FirstAsync();

            if (clearPassword.Length < passwordConfigurations.MinLength)
                brokenRules.Add(new BrokenRule("PASSWORD_LENGHT",
                    string.Format(ErrorDescriptions.PasswordLenght, passwordConfigurations.MinLength.ToString()), BrokenRule.BrokenRuleLevelEnum.Error));

            if (passwordConfigurations.SpecialCharacters.Length > 0
                && clearPassword.IndexOfAny(passwordConfigurations.SpecialCharacters) == -1)
                brokenRules.Add(new BrokenRule("PASSWORD_REQUIRED_SPECIAL_CHARS",
                        string.Format(ErrorDescriptions.PasswordRequiredSpecialChars, new string(passwordConfigurations.SpecialCharacters)), BrokenRule.BrokenRuleLevelEnum.Error));

            if (!clearPassword.Any(char.IsUpper))
                brokenRules.Add(new BrokenRule("PASSWORD_REQUIRED_UPPER_CHARS",
                    ErrorDescriptions.PasswordRequiredUpperChars, BrokenRule.BrokenRuleLevelEnum.Error));

            if (!clearPassword.Any(char.IsLower))
                brokenRules.Add(new BrokenRule("PASSWORD_REQUIRED_LOWER_CHARS",
                    ErrorDescriptions.PasswordRequiredLowerChars, BrokenRule.BrokenRuleLevelEnum.Error));
        }
        private string EncryptPassword(string clearPassword)
        {
            var asBytes = System.Text.Encoding.Unicode.GetBytes(clearPassword);

            return BitConverter.ToString(
                                SHA1CryptoServiceProvider.Create().ComputeHash(asBytes))
                            .Replace("-", string.Empty);
        }

        #endregion
    }
}