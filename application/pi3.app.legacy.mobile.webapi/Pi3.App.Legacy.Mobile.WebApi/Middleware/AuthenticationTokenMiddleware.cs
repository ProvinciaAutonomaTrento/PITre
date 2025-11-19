// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using System.Security.Claims;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Core.Services.Principal;

using Pi3.App.Legacy.Mobile.Shared.Extensions;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

using QUERY = Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Queries;
using DTO = Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs;
using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;
using MODELS = Pi3.App.Legacy.Mobile.Models;
using Newtonsoft.Json.Linq;
using Pi3.App.Legacy.Mobile.WebApi.Manager;
using Amazon.Runtime;
using Amazon.Auth.AccessControlPolicy;
using Pi3.App.Legacy.Mobile.WebApi.Services.AAC;

namespace Pi3.App.Legacy.Mobile.WebApi.Middleware;

public class AuthenticationTokenMiddleware(
    RequestDelegate next,
    ILogger<AuthenticationTokenMiddleware> logger,
    IServiceScopeFactory scopeFactory)
{
    readonly RequestDelegate _next = next;
    readonly ILogger<AuthenticationTokenMiddleware> _logger = logger;
    readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            string? headerTokenValue = context.Request.Headers["AuthToken"].FirstOrDefault();
            string? headerInstanceValue = context.Request.Headers["Instance"].FirstOrDefault();
            string? aacToken = string.Empty;

            if (!string.IsNullOrEmpty(headerTokenValue) && headerTokenValue.Length > 4)
            {
                string tokenValue = headerTokenValue[4..];
                //                string? infoUtenteToken = Shared.Helpers.AuthenticationHelper.DecryptToken(tokenValue);
                string? infoUtenteToken = Utils.Decrypt(tokenValue);

                string[] infoUtenteArray = infoUtenteToken.Split('|');
                string username = infoUtenteArray[5];
                string idAmministrazione_Token = infoUtenteArray[4];
                var ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(infoUtenteArray[0]);

                if (ruolo == null)
                {
                    throw new EXCEPTIONS.RequestParamNotFoundException("idGroup");
                }
                if (!long.TryParse(ruolo.idGruppo, out long idGroup))
                {
                    throw new EXCEPTIONS.RequestParamNotFoundException("idGroup");
                }
                if (!long.TryParse(idAmministrazione_Token, out long idAmministrazione))
                {
                    throw new EXCEPTIONS.RequestParamNotFoundException("IdAmministrazione");
                }

                using var scope = this._scopeFactory.CreateScope();
                IPi3DbContext dbContext = scope.ServiceProvider.GetRequiredService<IPi3DbContext>();
                IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                IMapper mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                IAacFetcherService _aacFetcherService = scope.ServiceProvider.GetRequiredService<IAacFetcherService>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var _options = configuration.GetSection("AACOptions").Get<AacOptions>();

                QUERY.GetUserQuery getUserQuery = new(username, idGroup, idAmministrazione);
                DTO.Users.GetUserDto utente = await mediator.Send(getUserQuery);

                //Token AAC
                var parms = new AacRequestBody()
                {
                    Scope = "client.roles.me",
                    ClientSecret = _options.ClientSecret,
                    ClientId = _options.ClientId,
                    GrantType = "client_credentials",
                };
                var aacResponse = await _aacFetcherService.GetToken(parms);
                aacToken = aacResponse.AccessToken;

                if (utente.IdPeople > 0)
                {
                    ClaimsIdentity claimsIdentity = GenerateIdentityFromUser(utente, headerInstanceValue, aacToken);
                    ClaimsPrincipal newPrincipal = new(claimsIdentity);
                    context.User = newPrincipal;
                }


            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "{Message}", ex.Message);
            throw;
        }

        await _next(context);
    }

    private static ClaimsIdentity GenerateIdentityFromUser(DTO.Users.GetUserDto utente, string? headerInstanceValue, string aacTokenValue)
    {
        List<Claim> claims =
            [
                new (Pi3ClaimTypes.Instance, headerInstanceValue),
                new (Pi3ClaimTypes.IdUser, utente.IdPeople.ToString()),
                new (Pi3ClaimTypes.UserId, utente.UserId!),
                new (Pi3ClaimTypes.UserName, utente.Nome ?? String.Empty),
                new (Pi3ClaimTypes.UserSurname, utente.Cognome ?? String.Empty),
                new (Pi3ClaimTypes.Admin, utente.IsAdmin.ToString()),
                new (Pi3ClaimTypes.IdGroup,utente.IdGruppo?.ToString() ?? String.Empty),
                new (Pi3ClaimTypes.GroupCode, utente.GroupCode ?? String.Empty),
                new (Pi3ClaimTypes.GroupDescription, utente.GroupDescription ?? String.Empty),
                new (Pi3ClaimTypes.IdTenant, utente.IdAmministrazione?.ToString() ?? String.Empty),
                new (Pi3ClaimTypes.TenantCode, utente.CodiceAmministrazione ?? String.Empty),
                new (Pi3ClaimTypes.TenantDescription, utente.DescrizioneAmministrazione ?? String.Empty),
                new (Pi3ClaimTypesExtended.IdCorrGlobali, utente.IdCorrGlobali?.ToString() ?? String.Empty),
                new ("KeyToken", $"Bearer {aacTokenValue}")
            ];

        foreach (string f in utente.Functions ?? [])
        {
            claims.Add(new Claim(Pi3ClaimTypes.Authorization, f));
        }

        ClaimsIdentity claimsIdentity = new(authenticationType: "Pi3Authentication", claims: claims);

        return claimsIdentity;
    }
}
