// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentAggregate
{
    public class VersionNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public VersionNotFoundPi3Exception(string idVersion)
            : base(ErrorDescriptions.VersionNotFound, ErrorDescriptions.ResourceManager, idVersion)
        {
            this.IdVersion = idVersion;
        }

        public string IdVersion { get; init; }

        #endregion
    }

}
