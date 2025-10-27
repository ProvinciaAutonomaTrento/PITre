// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories
{
    public class Pagination : ValueObject
    {
        public Pagination()
        { }

        public int? Skip { get; set; } = 0;

        public int? Take { get; set; } = 10;

        public static Pagination Default
        {
            get
            {
                return new Pagination();
            }
        }
    }

    public class GetAggregatoDocumentaleLoadBehavior : ValueObject, ILoadBehavior
    {
        public GetAggregatoDocumentaleLoadBehavior()
        {
        }

        public bool BypassSecurityCheck { get; init; } = false;

        public bool LoadDocuments { get; init; } = false;

        public Pagination? DocumentsPagination { get; init; } = null;

        public bool LoadFolderHierarchy { get; init; } = false;

        public Pagination? FoldersPagination { get; init; } = null;

        public bool LoadPermissions { get; init; } = true;

        public bool LoadProfiles { get; init; } = false;

        public bool LoadProfilesMetadata { get; init; } = false;

        public bool LoadNote { get; init; } = false;

        public bool LoadClassifications { get; init; } = true;
    }
}
