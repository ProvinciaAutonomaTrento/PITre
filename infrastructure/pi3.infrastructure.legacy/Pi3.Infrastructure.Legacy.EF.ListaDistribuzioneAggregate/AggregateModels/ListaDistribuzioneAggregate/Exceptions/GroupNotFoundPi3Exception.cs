// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.ListaDistribuzioneAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.ListaDistribuzioneAggregate.Exceptions
{
    public class GroupNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public GroupNotFoundPi3Exception(string idGroup)
            : base(ErrorDescriptions.GroupNotFound, null, ErrorDescriptions.ResourceManager, idGroup)
        {
            IdGroup = idGroup;
        }

        public string IdGroup { get; init; }

        #endregion
    }
}
