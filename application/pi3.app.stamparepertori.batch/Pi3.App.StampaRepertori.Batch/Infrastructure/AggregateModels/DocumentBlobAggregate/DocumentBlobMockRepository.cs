// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.Services.Configuration;

namespace Pi3.App.StampaRepertori.Batch.Infrastructure.AggregateModels.DocumentBlobAggregate
{
    public class DocumentBlobMockRepository : Pi3.Infrastructure.Legacy.DocumentFSRepository.AggregateModels.DocumentBlobAggregate.DocumentBlobFileSystemRepository
    {
        #region Public Members

        public DocumentBlobMockRepository(ILogger<DocumentBlobMockRepository> logger, IClaimsPrincipalService claimsPrincipalService, IEventPublisher eventPublisher, IConfigurationService configurationService)
            : base(logger, claimsPrincipalService, eventPublisher, configurationService)
        {
        }

        #endregion

        #region Private Members

        protected override Task<bool> HandleExists(string id)
        {
            return base.HandleExists(id);
        }

        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            return true;
        }

        protected override Task<DocumentBlob> HandleGet(DocumentBlob newAggregate, string id, params ILoadBehavior[] loadBehaviors)
        {
            return base.HandleGet(newAggregate, id, loadBehaviors);
        }

        protected override async Task<DocumentBlob> HandleGet(DocumentBlob newAggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            var fileName = $"{nameof(Files.Test_documento)}.pdf";

            var aggregate = new DocumentBlob(id, idTenant, DateTime.Now, new(fileName));
            aggregate.LoadFileName(fileName);
            aggregate.LoadStream(new MemoryStream(Files.Test_documento));
            aggregate.MarkChangesAsCommitted();
            return aggregate;
        }

        protected override async Task HandleAdd(DocumentBlob aggregate)
        {
            aggregate.AssignId(Guid.NewGuid().ToString());
        }

        #endregion
    }
}
