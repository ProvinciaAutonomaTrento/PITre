// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.WebApi.Application.Services.Principal;
using Pi3.App.Legacy.WebApi.Models;
using Pi3.App.Legacy.WebApi.Resources;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using System.IO.Pipelines;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pi3.App.Legacy.WebApi.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ErrorHandlerMiddleware>();
        }

        public async Task Invoke(HttpContext context, IClaimsPrincipalService claimsPrincipalService)
        {
           
            try
            {
                await _next(context);
            }
            catch (Pi3Exception pi3Ex)
            {

                this._logger.LogError(pi3Ex, await GetClaimsInformations(claimsPrincipalService));

                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)pi3Ex.StatusCode;

                await response.WriteAsync(JsonSerializer.Serialize(new ExceptionInfo()
                {
                    ErrorCode = pi3Ex.ErrorCode,
                    Message = pi3Ex.Message
                }));
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, await GetClaimsInformations(claimsPrincipalService));

                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await response.WriteAsync(JsonSerializer.Serialize(new ExceptionInfo()
                {
                    ErrorCode = ErrorDescriptions.UnhandledError,
                    Message = ex.Message
                }));
            }
        }

        private async Task<string> GetClaimsInformations(IClaimsPrincipalService claimsPrincipalService)
        {
            ClaimsPrincipal current = null!;

            try
            {
                current = claimsPrincipalService.Current;
            }
            catch (UnauthorizedPi3Exception)
            { }
            var idUser = current?.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
            var userId = current?.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var idGroup = current?.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup);
            var groupCode = current?.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode);
            var idTenant = current?.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            var tenantCode = current?.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode);
            var instance = current?.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance);

            return $"claimsPrincipalInformations - IdUser: {idUser}; UserId: '{userId}'; IdGroup: {idGroup}; GroupCode: '{groupCode}'; IdTenant: {idTenant}; TenantCode: '{tenantCode}'; Instance: '{instance}'";

        }
    }
}
