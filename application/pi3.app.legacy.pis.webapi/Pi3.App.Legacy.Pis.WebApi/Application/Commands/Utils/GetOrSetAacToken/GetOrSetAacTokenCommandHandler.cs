// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Aac;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.DistributedCache;
using Pi3.Core.Extensions;
using RabbitMQ.Client;
using Refit;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils.GetOrSetAacToken
{
    public class GetOrSetAacTokenCommandHandler : IRequestHandler<GetOrSetAacTokenCommand, GetOrSetAacTokenCommandResponse>
    {
        public GetOrSetAacTokenCommandHandler(IAacFetcherService _aacFetcherService, IDistributedCacheService _distributedCacheService, IConfiguration configuration, ILogger<GetOrSetAacTokenCommandHandler> logger)
        {
            this._options = configuration.GetSection("AACOptions").Get<AacOptions>();
            this._aacFetcherService = _aacFetcherService;
            this._distributedCacheService = _distributedCacheService;
            this._logger = logger;
            this._cacheExpInMinutes = configuration.GetSection("DistributedCacheServiceOptions:ExpireCache").Value ?? string.Empty;
        }

        public async Task<GetOrSetAacTokenCommandResponse> Handle(GetOrSetAacTokenCommand request, CancellationToken cancellationToken)
        {
            string token = string.Empty;
            try
            {
                token = await this._distributedCacheService.GetVal(new() 
                {
                    Key = Resource.KeyToken
                });
                if (string.IsNullOrEmpty(token))
                {
                    var parms = new AacRequestBody()
                    {
                        Scope = Resource.Scope,
                        ClientSecret = this._options.ClientSecret,
                        ClientId = this._options.ClientId,
                        GrantType = Resource.GrantType,
                    };
                    var aacResponse = await this._aacFetcherService.GetToken(parms);
                    token = aacResponse.AccessToken;
                    if (!string.IsNullOrEmpty(token))
                    {
                        Dictionary<string, string> cacheParams = new()
                        {
                            { "key" , Resource.KeyToken},
                            { "value" , token},
                            { "expiresInMinutes" , this._cacheExpInMinutes},

                        };
                        await this._distributedCacheService.PutVal(cacheParams);
                    }
                }
            }
            catch (Exception ex)
            {
                token = string.Empty;
                this._logger.LogError(exception: ex, message: ex.Message);
            }


            return new GetOrSetAacTokenCommandResponse() 
            { 
                Token = token
            };
        }

        #region Private Members
        protected readonly IAacFetcherService _aacFetcherService;
        protected readonly IDistributedCacheService _distributedCacheService;
        protected readonly AacOptions _options;
        protected readonly ILogger<GetOrSetAacTokenCommandHandler> _logger;
        protected readonly string _cacheExpInMinutes;

        protected class AacOptions
        {
            public string GrantType { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string Scope { get; set; }
            public string BaseUrl { get; set; }
        }
        #endregion
    }
}
