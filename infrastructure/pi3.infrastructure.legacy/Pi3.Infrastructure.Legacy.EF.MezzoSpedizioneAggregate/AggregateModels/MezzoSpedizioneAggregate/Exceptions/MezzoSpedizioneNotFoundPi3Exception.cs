// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.MezzoSpedizioneAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.MezzoSpedizioneAggregate.Exceptions
{
    public class MezzoSpedizioneNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public MezzoSpedizioneNotFoundPi3Exception(string id)
            : base(ErrorDescriptions.MezzoSpedizioneNonTrovato, null, ErrorDescriptions.ResourceManager, id)
        {
            Id = id;
        }
        public string Id { get; init; }

        #endregion
    }
}
