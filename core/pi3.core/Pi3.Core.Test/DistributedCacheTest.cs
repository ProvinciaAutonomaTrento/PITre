// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    public class DistributedCacheTest
    {

        IServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddDistributedMemoryCache()
                .BuildServiceProvider();
        }

        [Test]
        public async Task HandleCacheTest()
        {
            var distributedCache = _serviceProvider.GetService<IDistributedCache>();

            var cached = await distributedCache.FromCache<ExampleValueObject>(
                async () =>
                {
                    return new ExampleValueObject()
                    {
                        StringProperty = "TestProperty"
                    };
                },
                "Key1");

            var cached2 = await distributedCache.FromCache<ExampleValueObject>(
                async () => 
                {
                    return new ExampleValueObject()
                    {
                        StringProperty = "TestProperty"
                    };
                },
                "Key1");
        }

        public class ExampleValueObject : ValueObject
        {
            public ExampleValueObject()
            { }

            [Required(AllowEmptyStrings = false)]
            public string StringProperty { get; init; }
        }
    }
}
