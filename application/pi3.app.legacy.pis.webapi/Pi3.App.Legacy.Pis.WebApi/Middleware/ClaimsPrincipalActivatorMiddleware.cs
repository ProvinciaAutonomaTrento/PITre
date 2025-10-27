// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.App.Legacy.Pis.WebApi.Resources;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Security.Claims;
using System.Security.Principal;

namespace Pi3.App.Legacy.Pis.WebApi.Middleware
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
            // Esclusione dei metodi di routing
            if (context.Request.Headers.ContainsKey("AuthToken") && !context.Request.Headers.ContainsKey("ROUTED_ACTION"))
            {
                if (!context.Request.Headers.ContainsKey("Instance"))
                    throw new BadRequestPi3Exception(ErrorDescriptions.MissingAuthorizationParameter, ErrorDescriptions.ResourceManager, "instance");
                try
                {
                    var tokenHeader = context.Request.Headers["AuthToken"].ToString();

                    string authInfoString = RestUtils.Decrypt(tokenHeader.Substring(4));
                    string[] authInfoArray = authInfoString.Split('|');

                    var dbContext = context.RequestServices.GetService<IPi3DbContext>();

                    var userContext = await
                                 (from p in dbContext.PeopleEntities.AsNoTracking()
                                  join a in dbContext.AmministraEntities.AsNoTracking() on p.ID_AMM equals a.SYSTEM_ID
                                  join cg in dbContext.CorrGlobaliEntities.AsNoTracking() on p.SYSTEM_ID equals cg.ID_PEOPLE
                                  join pg in dbContext.PeopleGroupEntities.AsNoTracking() on p.SYSTEM_ID equals pg.PEOPLE_SYSTEM_ID
                                  join g in dbContext.GroupEntities.AsNoTracking() on pg.GROUPS_SYSTEM_ID equals g.SYSTEM_ID
                                  join cg1 in dbContext.CorrGlobaliEntities.AsNoTracking() on g.SYSTEM_ID equals cg1.ID_GRUPPO
                                  where
                                     a.SYSTEM_ID == authInfoArray[4].AsLong()
                                     && p.USER_ID == authInfoArray[5]
                                     && g.SYSTEM_ID == authInfoArray[2].AsLong()
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

                    if (userContext != null && userContext.PEOPLE_SYSTEM_ID > 0)
                    {
                        string instance = string.Empty;
                        Microsoft.Extensions.Primitives.StringValues instHeader = string.Empty;
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

                        context.Request.Headers.TryGetValue("Instance", out instHeader);
                        if (instHeader != StringValues.Empty)
                            instance = instHeader.FirstOrDefault() ?? "";

                        claims.Add(new Claim(Pi3ClaimTypes.Instance, instance));

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
                    }
                }catch(Exception ex)
                {
                    throw new BadRequestPi3Exception(ErrorDescriptions.InvalidAuthToken, ErrorDescriptions.ResourceManager, "instance");
                }
            }
            await _next(context);
        }
    }
}
