// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.Services.Principal;
using Pi3.Core;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using Pi3.Infrastructure.Legacy.EF.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Oracle;
using Pi3.Infrastructure.Legacy.DocumentFSRepository;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Security.Claims;

namespace Pi3.Infrastructure.Legacy.Test
{
    [Ignore("Ignorati temporaneamente")]
    public class DistributedCacheTests
    {
        ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                .BuildServiceProvider();
        }

        [Test]
        public async Task HandleCacheTest()
        {
            var claimsPrincipalService = _serviceProvider.GetRequiredService<IClaimsPrincipalService>();
            var instance = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, true);

            var distributedCache = _serviceProvider.GetRequiredService<IDistributedCache>();
            var dbContext = _serviceProvider.GetRequiredService<IPi3DbContext>();

            var cached = await distributedCache.FromCache<ChiaveConfigurazioneEntity>(
                instance!,
                dbContext.ChiaviConfigurazioneEntities);

            cached = await distributedCache.FromCache<ChiaveConfigurazioneEntity>(
                instance!,
                dbContext.ChiaviConfigurazioneEntities);

            cached = await distributedCache.FromCache<ChiaveConfigurazioneEntity>(
                instance!,
                dbContext.ChiaviConfigurazioneEntities,
                c => c.CHA_GLOBALE == "1");

            var ragioni = await distributedCache.FromCache<RagioneTrasmissioneEntity>(
                instance!,
                dbContext.RagioneTrasmissioneEntities);

            var amm = await distributedCache.FromCache<AmministrazioneEntity>(
                instance!,
                dbContext.AmministraEntities, 
                a => a.SYSTEM_ID == 22);

            var cached2 = await distributedCache.FromCache<ChiaveConfigurazioneEntity>(
                instance!,
                dbContext.ChiaviConfigurazioneEntities);

            var cached3 = await distributedCache.FromCache<ChiaveConfigurazioneEntity>(
                instance!,
                dbContext.ChiaviConfigurazioneEntities, 
                c => c.VAR_CODICE == "FE_TIMER_DISSERVIZIO");

            var cached4 = await distributedCache.FromCache<ChiaveConfigurazioneEntity>(
                instance!,
                dbContext.ChiaviConfigurazioneEntities, 
                c => c.VAR_CODICE == "FE_TIMER_DISSERVIZIO");

            Assert.True(cached != null);
        }
    }
}
