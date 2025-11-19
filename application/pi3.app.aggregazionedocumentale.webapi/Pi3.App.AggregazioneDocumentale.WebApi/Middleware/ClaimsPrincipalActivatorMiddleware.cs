// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Security.Claims;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Middleware
{
    public class ClaimsPrincipalActivatorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ClaimsPrincipalActivatorMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ClaimsPrincipalActivatorMiddleware>();
        }

        public async Task Invoke(HttpContext context,
            ILogger<ClaimsPrincipalActivatorMiddleware> logger)
        {
            if (!context.Request.Headers.ContainsKey("tenant"))
                throw new BadRequestPi3Exception(ErrorDescriptions.MissingAuthorizationParameter, ErrorDescriptions.ResourceManager, "tenant");

            if (!context.Request.Headers.ContainsKey("userId"))
                throw new BadRequestPi3Exception(ErrorDescriptions.MissingAuthorizationParameter, ErrorDescriptions.ResourceManager, "userId");

            if (!context.Request.Headers.ContainsKey("groupCode"))
                throw new BadRequestPi3Exception(ErrorDescriptions.MissingAuthorizationParameter, ErrorDescriptions.ResourceManager, "groupCode");

            var codiceAmministrazione = context.Request.Headers["tenant"].ToString().ToUpperInvariant();
            var userId = context.Request.Headers["userId"].ToString().ToUpperInvariant();
            var groupCode = context.Request.Headers["groupCode"].ToString().ToUpperInvariant();

            var dbContext = context.RequestServices.GetService<IPi3DbContext>();

            var userContext = await
                         (from p in dbContext.PeopleEntities.AsNoTracking()
                          join a in dbContext.AmministraEntities.AsNoTracking() on p.ID_AMM equals a.SYSTEM_ID
                          join cg in dbContext.CorrGlobaliEntities.AsNoTracking() on p.SYSTEM_ID equals cg.ID_PEOPLE
                          join pg in dbContext.PeopleGroupEntities.AsNoTracking() on p.SYSTEM_ID equals pg.PEOPLE_SYSTEM_ID
                          join g in dbContext.GroupEntities.AsNoTracking() on pg.GROUPS_SYSTEM_ID equals g.SYSTEM_ID
                          join cg1 in dbContext.CorrGlobaliEntities.AsNoTracking() on g.SYSTEM_ID equals cg1.ID_GRUPPO
                          where
                             a.VAR_CODICE_AMM == codiceAmministrazione
                             && p.USER_ID == userId
                             && g.GROUP_ID == groupCode
                             && pg.DTA_FINE == null
                          select new
                          {
                              PEOPLE_SYSTEM_ID = p.SYSTEM_ID,
                              PEOPLE_USER_ID = p.USER_ID,
                              PEOPLE_VAR_COGNOME = p.VAR_COGNOME,
                              PEOPLE_VAR_NOME = p.VAR_NOME,
                              PEOPLE_CHA_AMMINISTRATORE = p.CHA_AMMINISTRATORE,
                              AMM_SYSTEM_ID = a.SYSTEM_ID,
                              AMM_VAR_CODICE_AMM = a.VAR_CODICE_AMM,
                              AMM_VAR_DESC_AMM = a.VAR_DESC_AMM,
                              CORRGLOBALI_SYSTEM_ID = cg.SYSTEM_ID,
                              GROUPS_SYSTEM_ID = g.SYSTEM_ID,
                              GROUPS_GROUP_ID = g.GROUP_ID,
                              GROUPS_GROUP_NAME = g.GROUP_NAME,
                              CORRGLOBALI_GROUPS_SYSTEM_ID = cg1.SYSTEM_ID,
                          })
                         .FirstOrDefaultAsync();

            if (userContext == null)
                throw new UnauthorizedPi3Exception(ErrorDescriptions.RequestNotAuthorized, ErrorDescriptions.ResourceManager);

            var claims = new List<Claim>();

            claims.Add(new Claim(Pi3ClaimTypes.IdUser, userContext.PEOPLE_SYSTEM_ID.ToString()));
            claims.Add(new Claim(Pi3ClaimTypes.UserId, userContext.PEOPLE_USER_ID));
            claims.Add(new Claim(Pi3ClaimTypes.UserName, userContext.PEOPLE_VAR_NOME));
            claims.Add(new Claim(Pi3ClaimTypes.UserSurname, userContext.PEOPLE_VAR_COGNOME));
            claims.Add(new Claim(Pi3ClaimTypes.Admin, (userContext.PEOPLE_CHA_AMMINISTRATORE == "1").ToString()));
            claims.Add(new Claim(Pi3ClaimTypes.IdGroup, userContext.GROUPS_SYSTEM_ID.ToString()));
            claims.Add(new Claim(Pi3ClaimTypes.GroupCode, userContext.GROUPS_GROUP_ID));
            claims.Add(new Claim(Pi3ClaimTypes.GroupDescription, userContext.GROUPS_GROUP_NAME));
            claims.Add(new Claim(Pi3ClaimTypes.IdTenant, userContext.AMM_SYSTEM_ID.ToString()));
            claims.Add(new Claim(Pi3ClaimTypes.TenantCode, userContext.AMM_VAR_CODICE_AMM));
            claims.Add(new Claim(Pi3ClaimTypes.TenantDescription, userContext.AMM_VAR_DESC_AMM));
            claims.Add(new Claim(Pi3ClaimTypes.Instance, context.GetRouteData().Values["instance"].ToString()));

            claims.AddRange(await (from tfr in dbContext.TipoFRuoloEntities.AsNoTracking()
                               join tf in dbContext.TipoFunzioneEntities.AsNoTracking() on tfr.ID_TIPO_FUNZ equals tf.SYSTEM_ID
                               join f in dbContext.FunzioneEntities.AsNoTracking() on tf.SYSTEM_ID equals f.ID_TIPO_FUNZIONE
                               where tfr.ID_RUOLO_IN_UO == userContext.CORRGLOBALI_GROUPS_SYSTEM_ID
                               select f.COD_FUNZIONE)
                        .Distinct()
                        .Select(f => new Claim(Pi3ClaimTypes.Authorization, f))
                        .ToListAsync());

            var claimsIdentity = new ClaimsIdentity(
                authenticationType: "Pi3Authentication",
                claims: claims);

            context.User.AddIdentity(claimsIdentity);

            await _next(context);
        }
    }
}
