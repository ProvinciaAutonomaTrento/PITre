// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Conservazione.Batch.Tests
{
    public class ConservazioneServiceTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(Files.Variables);

            variables?.ToList().ForEach(v => Environment.SetEnvironmentVariable(v.Key, v.Value));
        }

        [Test, Order(1)]
        public async Task RunnerTestExecutePolicy()
        {
            var instance = Environment.GetEnvironmentVariable("Instance");

            await Runner.Run(new string[2] {$"--Instance={instance}", "0"});

            Assert.Pass();
        }

        [Test, Order(2)]
        public async Task RunnerTestExecuteDailyErrorReports()
        {
            var instance = Environment.GetEnvironmentVariable("Instance");

            await Runner.Run(new string[2] { $"--Instance={instance}", "1" });

            Assert.Pass();
        }

        [Test, Order(3)]
        public async Task RunnerTestExecuteVersamento()
        {
            var instance = Environment.GetEnvironmentVariable("Instance");

            await Runner.Run(new string[2] { $"--Instance={instance}", "2" });

            Assert.Pass();
        }
    }
}
