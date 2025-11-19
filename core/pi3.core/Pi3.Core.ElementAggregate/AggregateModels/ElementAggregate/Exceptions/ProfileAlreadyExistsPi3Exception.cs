// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ElementAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ElementAggregate.Exceptions
{
    public class ProfileAlreadyExistsPi3Exception : Pi3Exception
    {
        #region Public Members

        public ProfileAlreadyExistsPi3Exception(string idProfile)
            : base(ErrorDescriptions.ProfileFieldNotFound, ErrorDescriptions.ResourceManager, idProfile)
        {
            IdProfile = idProfile;
        }

        public string IdProfile { get; init; }

        #endregion
    }
}
