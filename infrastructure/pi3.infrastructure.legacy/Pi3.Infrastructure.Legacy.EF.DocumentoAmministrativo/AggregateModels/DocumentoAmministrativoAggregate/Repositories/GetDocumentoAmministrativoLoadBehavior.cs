// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories
{
  
    public class GetDocumentoAmministrativoLoadBehavior : ValueObject, ILoadBehavior
    {
        public GetDocumentoAmministrativoLoadBehavior()
        {
        }

        public bool BypassSecurityCheck { get; init; } = false;

        public bool LoadVersions { get; init; } = true;

        public Pagination? VersionsPagination { get; init; } = null;

        public bool LoadRelatedElements { get; init; } = true;

        public bool LoadPermissions { get; init; } = true;

        public bool LoadAggregazioni { get; init; } = true;

        public Pagination? AggregazioniPagination { get; init; } = null;

        public bool LoadClassifications { get; init; } = true;

        public Pagination? ClassificationsPagination { get; init; } = null;

        public bool LoadProfiles { get; init; } = true;

        public bool LoadProfilesMetadata { get; init; } = false;

        public bool LoadAllegati { get; init; } = true;

        public Pagination? AllegatiPagination { get; init; } = null;

        public bool LoadMittentiDestinatari { get; init; } = true;

        public Pagination? MittentiDestinatariPagination { get; init; } = null;

        public bool LoadNote { get; init; } = true;

        public bool LoadKeywords { get; init; } = true;
    }
}
