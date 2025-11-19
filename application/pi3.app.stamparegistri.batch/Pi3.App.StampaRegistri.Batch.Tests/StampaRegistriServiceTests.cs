// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.StampaRegistri.Batch.Tests
{
    public class StampaRegistriServiceTests
    {
        [SetUp]
        public void Setup()
        {
            var variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(Files.Variables);

            variables?.ToList().ForEach(v => Environment.SetEnvironmentVariable(v.Key, v.Value));
        }

        [Test]
        public async Task RunnerTest()
        {
            await Runner.Run(Array.Empty<string>());

            Assert.Pass();
        }
    }
}
