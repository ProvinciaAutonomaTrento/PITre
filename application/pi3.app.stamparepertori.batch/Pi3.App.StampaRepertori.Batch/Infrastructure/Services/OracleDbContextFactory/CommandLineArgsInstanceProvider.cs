// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.StampaRepertori.Batch.Infrastructure.Services.OracleDbContextFactory
{
    public class CommandLineArgsInstanceProvider : IInstanceProvider
    {
        public CommandLineArgsInstanceProvider()
        {
                
        }

        public string Instance
        {
            get
            {
                var argsList = Environment.GetCommandLineArgs().ToList();

                var instance = argsList.FirstOrDefault(a => a.ToUpper().Contains("INSTANCE"));

                if (instance is null) throw new InstanceNotFoundPi3Exception();

                return instance;
            }
        }
    }
}
