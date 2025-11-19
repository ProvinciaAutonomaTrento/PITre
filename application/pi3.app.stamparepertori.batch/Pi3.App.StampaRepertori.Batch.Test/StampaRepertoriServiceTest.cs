// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Newtonsoft.Json;

namespace Pi3.App.StampaRepertori.Batch.Test
{
    public class StampaRepertoriServiceTest
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
            await Runner.Run(new string[0]);

            Assert.Pass();
        }
    }
}