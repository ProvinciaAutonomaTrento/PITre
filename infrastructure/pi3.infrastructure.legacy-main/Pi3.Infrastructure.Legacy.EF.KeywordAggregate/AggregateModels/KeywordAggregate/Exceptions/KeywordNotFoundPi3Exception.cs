// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.KeywordAggregate.Resources;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.KeywordAggregate.Exceptions
{
    public class KeywordNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public KeywordNotFoundPi3Exception(string id)
           : base(ErrorDescriptions.KeywordNotFound, null, ErrorDescriptions.ResourceManager, id)
        {
            Id = id;
        }
        public string Id { get; init; }

        #endregion
    }

}
