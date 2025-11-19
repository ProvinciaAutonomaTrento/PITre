// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class FunctionGroupTest
    {
        IServiceProvider _serviceProvider;
        IElementRepository<FunctionGroup> _repository;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<FunctionGroup>, InMemoryElementRepository<FunctionGroup>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetService<IElementRepository<FunctionGroup>>();
        }

        [Test]
        public async Task AddFunctionGroupTest()
        {
            var function = new FunctionGroup(Guid.NewGuid().ToString(), "IdTenant", DateTime.Now, new SeedWork.TextValue("FUNCTION 1"), new SeedWork.TextValue("DESCRIPTION FUNCTION 1"));

            function.AddFunction(new Function("1", "DO_PROTOCOLLA", new SeedWork.TextValue("Funzione per protocollare", new SeedWork.MultiLanguageTextValue("en", "Function to protocol"))));
            function.AddFunction(new Function("2", "DO_CREA_DOCUMENTO", new SeedWork.TextValue("Funzione per creare documento", new SeedWork.MultiLanguageTextValue("en", "Function to protocol"))));

            await _repository.Add(function);

            function.AddFunction(new Function("3", "DO_ANY", new SeedWork.TextValue("Funzione per creare documento", new SeedWork.MultiLanguageTextValue("en", "Function to protocol"))));
            function.RemoveFunction(function.Functions.First(f => f.Code == "DO_PROTOCOLLA"));

            function.ChangeDescription(new SeedWork.TextValue("PROVA"));

            await _repository.Update(function);
        }
    }
}
