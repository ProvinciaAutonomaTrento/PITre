// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Mvc.Filters;
using Pi3.App.Legacy.WebApi.Application.Services.Principal;
using Pi3.App.Legacy.WebApi.Requests;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using System.Security.Claims;

namespace Pi3.App.Legacy.WebApi.ActionFilters
{
    public class ClaimsPrincipalActivatorActionFilter : IAsyncActionFilter
    {
        #region Public Members

        public ClaimsPrincipalActivatorActionFilter(
            ILogger<ClaimsPrincipalActivatorActionFilter> logger,
            IClaimsPrincipalService claimsPrincipalService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context, ActionExecutionDelegate next)
        {            
            if (context.ActionArguments.Any() && context.ActionArguments.ContainsKey("request"))
            {
                var request = context.ActionArguments["request"] as DataPortalRequest;
                
                if (request != null && request.PrincipalContext != null)
                {
                    var identity = new ClaimsIdentity(request.PrincipalContext.AuthenticationType);
                    
                    identity.AddClaim(new Claim(Pi3ClaimTypes.Instance, context.RouteData.Values["instance"].ToString()));
                    identity.AddClaim(new Claim("KeyToken", context.HttpContext.Request.Headers["Authorization"].ToString()));

                    identity.AddClaims(request.PrincipalContext
                        .Claims
                        .Select(c => new Claim(c.Name, c.Value, c.ValueType, c.Issuer)));

                    context.HttpContext.User.AddIdentity(identity);
                    ((ClaimsPrincipalService) this._claimsPrincipalService).Current = context.HttpContext.User;
                }
            }

            await next();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ClaimsPrincipalActivatorActionFilter> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly string BearerPrefix = "Bearer ";

        #endregion
    }
}
