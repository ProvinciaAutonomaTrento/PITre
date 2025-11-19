// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Gmail.v1;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using System.Net.Http;

namespace Pi3.App.Legacy.WebApi.Application.Services.Google;

public class AuthProvider
{
    private readonly string _clientId;
    private readonly string _clientSecret;

    public AuthProvider(string clientId, string clientSecret)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    public string GetAuthorizationUrl(string redirectUri)
    {
        var clientSecrets = new ClientSecrets
        {
            ClientId = _clientId,
            ClientSecret = _clientSecret
        };

        var authorizationCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = clientSecrets,
            Scopes = new[] { GmailService.Scope.GmailModify }
        });

        var authorizationUrl = authorizationCodeFlow.CreateAuthorizationCodeRequest(redirectUri).Build();
        return authorizationUrl.ToString();
    }

    public async Task<UserCredential> GetCredentialWithCodeAsync(string code, string redirectUri)
    {
        var clientSecrets = new ClientSecrets
        {
            ClientId = _clientId,
            ClientSecret = _clientSecret
        };

        using var httpClient = new HttpClient();
        var tokenResponse = await new AuthorizationCodeTokenRequest
        {
            Code = code,
            ClientId = _clientId,
            ClientSecret = _clientSecret,
            RedirectUri = redirectUri
        }.ExecuteAsync(httpClient, "https://oauth2.googleapis.com/token", CancellationToken.None, SystemClock.Default);

        var credential = new UserCredential(
            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets
            }),
            "user",
            tokenResponse);

        return credential;
    }
}