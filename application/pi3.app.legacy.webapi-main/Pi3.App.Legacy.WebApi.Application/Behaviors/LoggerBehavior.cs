// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace Pi3.App.Legacy.WebApi.Application.Behaviors
{
    public class LoggerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggerBehavior<TRequest, TResponse>> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;

        public LoggerBehavior(ILogger<LoggerBehavior<TRequest, TResponse>> logger, IClaimsPrincipalService claimsPrincipalService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
        }
        

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            TResponse response;

            ClaimsPrincipal current = null!;

            try
            {
                current = this._claimsPrincipalService.Current;
            }
            catch (UnauthorizedPi3Exception unPi3Ex)
            {
                this._logger.LogDebug(unPi3Ex.Message);
            }

            var idUser = current?.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
            var userId = current?.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var idGroup = current?.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup);
            var groupCode = current?.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode);
            var idTenant = current?.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            var tenantCode = current?.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode);
            var instance = current?.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance);

            var startDate = DateTime.UtcNow;
            var withErrors = false;
            try
            {
                this._logger.LogInformation($"Handling;{typeof(TRequest).Name};{startDate.ToString()};{idUser};{userId};{idGroup};{groupCode};{idTenant};{tenantCode};{instance};");

                response = await next();
            }
            catch
            {
                withErrors = true;
                throw;
            }
            finally
            {
                var endDate = DateTime.UtcNow;
                
                this._logger.LogInformation($"Handled;{typeof(TRequest).Name};{endDate.Subtract(startDate).TotalSeconds};{endDate.AsDateTimeFormat()};{idUser};{userId};{idGroup};{groupCode};{idTenant};{tenantCode};{instance};{withErrors};");
            }

            return response;
        }
    }
}
