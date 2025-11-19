// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pi3.App.InteropPitre.WebApi.Models;
using Pi3.App.InteropPitre.WebApi.Resources;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Security.Claims;

namespace Pi3.App.InteropPitre.WebApi.Middleware
{
    public class ClaimsPrincipalActivatorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IOptions<SuperadminUserOptions> _superadminUserOptions;

        public ClaimsPrincipalActivatorMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<SuperadminUserOptions> superadminUserOptions)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ClaimsPrincipalActivatorMiddleware>();
            _superadminUserOptions = superadminUserOptions;
        }

        public async Task Invoke(HttpContext context,
            ILogger<ClaimsPrincipalActivatorMiddleware> logger)
        {
            if (!context.Request.Headers.ContainsKey("tenant"))
                throw new BadRequestPi3Exception(ErrorDescriptions.MissingAuthorizationParameter, ErrorDescriptions.ResourceManager, "tenant");

            var codiceAmministrazione = context.Request.Headers["tenant"].ToString().ToUpperInvariant();
            var superadminUserId = _superadminUserOptions.Value.UserId.ToUpperInvariant();

            var dbContext = context.RequestServices.GetService<IPi3DbContext>();

            var userEntity = await 
                            (from p in dbContext.PeopleEntities.AsNoTracking()
                                where p.USER_ID == superadminUserId
                                select new
                                {
                                    p.SYSTEM_ID,
                                    p.USER_ID
                                }).FirstOrDefaultAsync();

            if (userEntity == null)
                throw new UnauthorizedPi3Exception(ErrorDescriptions.RequestNotAuthorized, ErrorDescriptions.ResourceManager);

            _logger.LogInformation("Codice Amministrazione: " + codiceAmministrazione);

            var amministrazioneEntity = await
                (from a in dbContext.AmministraEntities.AsNoTracking()
                 where a.VAR_CODICE_AMM.ToUpper() == codiceAmministrazione.ToUpper()
                 select new
                 {
                     a.SYSTEM_ID,
                     a.VAR_CODICE_AMM,
                     a.VAR_DESC_AMM
                 })
                 .FirstAsync();

            var claims = new List<Claim>();

            claims.Add(new Claim(Pi3ClaimTypes.IdUser, userEntity.SYSTEM_ID.ToString()));
            claims.Add(new Claim(Pi3ClaimTypes.UserId, userEntity.USER_ID));
            claims.Add(new Claim(Pi3ClaimTypes.SuperAdmin, true.ToString()));
            claims.Add(new Claim(Pi3ClaimTypes.IdTenant, amministrazioneEntity.SYSTEM_ID.ToString()));
            claims.Add(new Claim(Pi3ClaimTypes.TenantCode, amministrazioneEntity.VAR_CODICE_AMM.ToString()));
            claims.Add(new Claim(Pi3ClaimTypes.TenantDescription, amministrazioneEntity.VAR_DESC_AMM.ToString()));

            var claimsIdentity = new ClaimsIdentity(
                authenticationType: "Pi3Authentication",
                claims: claims);

            context.User.AddIdentity(claimsIdentity);

            await _next(context);
        }
    }
}
