// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentBlobAggregate.Exceptions
{
    public class DocumentBlobPi3Exception : Pi3Exception
    {
        #region Public Members

        public DocumentBlobPi3Exception(string message, System.Resources.ResourceManager resourceManager, string id)
            : base(message, null, resourceManager, id)
        {
            Id = id;
        }

        public string Id { get; init; }

        #endregion
    }
}
