// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.Services.Configuration.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Services.Services.Configuration.Exceptions
{
    public class ConfigurationKeyNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public ConfigurationKeyNotFoundPi3Exception(string key)
            : base(ErrorDescriptions.ConfigurationKeyNotFound, null, ErrorDescriptions.ResourceManager, key)
        {
            Key = key;
        }

        public string Key { get; init; }

        #endregion
    }
}
