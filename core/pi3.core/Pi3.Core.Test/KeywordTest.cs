// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.KeywordAggregate;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class KeywordTest
    {
        IServiceProvider _serviceProvider;
        IElementRepository<Keyword> _repository;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<Keyword>, InMemoryElementRepository<Keyword>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetService<IElementRepository<Keyword>>();
        }

        [Test]
        public async Task AddKeywordPublicTest()
        {
            var aggregate = new Keyword(
                Guid.NewGuid().ToString(),
                "idTenant",
                DateTime.Now,
                new SeedWork.TextValue("Keyword_test"),
                new SeedWork.TextValue("")
               );

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null
                && aggregate.Name != null);
        }
        
    }
}
