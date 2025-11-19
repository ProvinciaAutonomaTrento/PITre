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
    public class MezzoSpedizioneDeleteNotSupportedPi3Exception : Pi3Exception
    {
        public MezzoSpedizioneDeleteNotSupportedPi3Exception(string Message)
        : base(ErrorDescriptions.MezzoDiSpedizioneGiaEliminato, null, ErrorDescriptions.ResourceManager)
        {
        }
    }
}
