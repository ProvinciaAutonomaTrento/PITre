// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Indexer.Infrastructure.AggregateModels.RequestAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Indexer.Infrastructure.AggregateModels.RequestAggregate.Exceptions
{

    public class RequestNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public RequestNotFoundPi3Exception(string idRequest)
            : base(ErrorDescriptions.RequestNotFound, null, ErrorDescriptions.ResourceManager)
        {
            IdRequest = idRequest;
        }

        public string IdRequest { get; init; }

        #endregion
    }

}
