// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetLogImportRubrica.Exceptions
{
    internal class ConfigurationKeyNotFound : Pi3.Core.SeedWork.Pi3Exception
    {
        public ConfigurationKeyNotFound() : 
            base(ErrorDescriptions.ConfigurationKeyNotFound, ErrorDescriptions.ResourceManager)
        { }

        public ConfigurationKeyNotFound(string key) :
            base(ErrorDescriptions.ConfigurationKeyWithNameNotFound, ErrorDescriptions.ResourceManager, key)
        { }
    }
}
