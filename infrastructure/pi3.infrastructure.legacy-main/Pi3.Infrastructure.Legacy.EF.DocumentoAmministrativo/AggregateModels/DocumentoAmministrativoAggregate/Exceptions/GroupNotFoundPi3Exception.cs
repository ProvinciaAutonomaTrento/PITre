// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Exceptions
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
