// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Conservazione.Batch.Infrastructure.Services.OracleDbContextFactory
{
    public class MockCommandLineArgsInstanceProvider : IInstanceProvider
    {
        public MockCommandLineArgsInstanceProvider()
        {
                
        }
        public string Instance
        {
            get
            {
                var instance = Environment.GetEnvironmentVariable("Instance");

                if (instance is null) throw new InstanceNotFoundPi3Exception();

                return instance;
            }
        }
    }
}
